using System.Data;
using Dapper;
using OfficialMemo.Models.Xml;

namespace OfficialMemo.Services;

public class TasksService
{
    private readonly IDbConnection _connection;
    private readonly ProcessService _processService;
    private readonly ProcessesDbService _processesDbService;

    public TasksService(IDbConnection connection, ProcessService processService, ProcessesDbService processesDbService)
    {
        _connection = connection;
        _processService = processService;
        _processesDbService = processesDbService;
    }

    class Task1
    {
        public Guid TaskGuid { get; set; }
        public Guid? ParentGuid { get; set; }
        public Guid RequestGuid { get; set; }
        public Guid? PreviousTaskGuid { get; set; }
        public bool IsRedirect { get; set; }
        public bool IsOpen { get; set; }
        public Task1? Parent { get; set; }
        public List<Task1>? Children { get; set; }
    }

    public async Task CloseTaskTreePathFromLeaf(Guid leafTask)
    {
        var tasksToClose = await GetTasksToClose(leafTask);

        if (tasksToClose.Count == 0)
            return;

        await CloseTasks(
            tasksToClose.Where(task => !task.IsRedirect).Select(task => task.TaskGuid)
        );
        var requestGuid = tasksToClose.First().RequestGuid;
        if (await _processesDbService.IsExistsRequest(requestGuid))
        {
            await ForwardRequest(tasksToClose.First().RequestGuid);
        }
    }

    public async Task UpdateSubrequests(Guid processGuid, string taskType, string[] performers)
    {
        var performerTasks = (
            await _connection.QueryAsync<(Guid TaskGuid, Guid? PreviousTaskGuid, string TaskStatus, string Login, Guid RequestGuid)>(
                """
                select task_guid as TaskGuid,
                       previous_task_guid as PreviousTaskGuid,
                       task_status as TaskStatus,
                       responsible as Login,
                       request_guid as RequestGuid
                from ABProcSystem.dbo.TASK_new with (nolock) where process_guid = @processGuid and task_type = @taskType
               """,
                new { processGuid, taskType }
            )
        ).ToList();

        var performersToAdd = performers.Except(performerTasks.Where(p => p.TaskStatus != "canceled").Select(p => p.Login)).ToList();
        var performersToRemove = performerTasks
            .Where(t => t.TaskStatus == "toPerform")
            .ExceptBy(performers, tuple => tuple.Login)
            .ExceptBy(performerTasks.Select(z => z.PreviousTaskGuid), tuple => tuple.TaskGuid)
            .Select(p => p.TaskGuid)
            .ToList();

        await AddSubrequests(processGuid, taskType, performersToAdd);
        await RemoveTasks(performersToRemove, performerTasks.FirstOrDefault().RequestGuid);
    }

    private async Task RemoveTasks(List<Guid> taskGuids, Guid requestGuid)
    {
        if (taskGuids.Any())
        {
            const string q = """
                 declare @taskGuidsToDelete table (task_guid uniqueidentifier);
                 with DeletedTasks as (
                     select task_guid, parent_guid, task_status from ABProcSystem.dbo.TASK_new with (nolock)
                     where task_guid in @taskGuids
                     union all select t.task_guid, t.parent_guid, t.task_status
                     from ABProcSystem.dbo.TASK_new as t with (nolock)
                     inner join DeletedTasks as d on d.task_guid = t.parent_guid
                 )
                 insert into @taskGuidsToDelete (task_guid)
                 select task_guid from DeletedTasks
                 where task_status in ('toPerform', 'toApprove');

                 delete tq from ABProcSystem.dbo.TASK_Q_new as tq
                 inner join @taskGuidsToDelete as d on d.task_guid = tq.task_guid;

                 update t
                 set t.task_status = 'canceled',
                     t.replied_by = getDate()
                 from ABProcSystem.dbo.TASK_new as t
                 inner join @taskGuidsToDelete as d on t.task_guid = d.task_guid;
             """;

            await _connection.ExecuteAsync(q, new { taskGuids });
            await ForwardRequest(requestGuid);
        }
        
    }

    public async Task UpdateDueToDateTasks(Guid requestGuid, DateTime dueToDate)
    {
        const string q = """
                update [ABProcsystem].[dbo].TASK_new
                    set due_to_date = cast(@dueToDate as datetime2)
                where request_guid = @requestGuid and task_type like 'subrequest%';

                update [ABProcsystem].[dbo].TASK_Q_new
                    set due_to_date = cast(@dueToDate as datetime2)
                where request_guid = @requestGuid and task_type like 'subrequest%';
            """;

        await _connection.ExecuteAsync(q, new { dueToDate, requestGuid });
    }

    private async Task AddSubrequests(Guid processGuid, string taskType, IEnumerable<string> performers)
    {
        var performersList = performers.ToList();
        if (!performersList.Any())
            return;

        const string q = """
            create table #Employees (login nvarchar(200));
            insert into #Employees (login) select value from string_split(@logins, ',')
            insert into ABProcSystem.dbo.TASK_new (task_guid, parent_guid, previous_task_guid, process_guid,
                              request_guid, approval_required, task_type, [initiator], responsible, approver,
                              task_date, task_name, task_comment, due_to_date, [data], child_count, task_status)
            select
                newid(), t.parent_guid, t.previous_task_guid, t.process_guid, t.request_guid, t.approval_required,
                t.task_type, system_user, e.login AS responsible, t.approver, getDate(), t.task_name, t.task_comment,
                t.due_to_date, '{}' as [data], 0 as child_count, 'toPerform' as task_status
            from (select top 1 * from ABProcSystem.dbo.TASK_new with (nolock) where process_guid = @processGuid and task_type = @taskType) t
            cross join #Employees e
            """;

        await _connection.ExecuteAsync(
            q,
            new { processGuid, taskType, logins = string.Join(",", performersList) }
        );
    }
    private async Task<List<Task1>> GetTasksToClose(Guid leafTask)
    {
        const string queryAllTasks = """
            select t.task_guid   as TaskGuid,
                   t.parent_guid as ParentGuid,
                   t.request_guid as RequestGuid,
                   t.previous_task_guid as PreviousTaskGuid,
                   cast (iif(t.task_status ='redirected', 1, 0) as bit) as IsRedirect,
                   cast(iif(q.task_guid is null, 0, 1) as bit ) as IsOpen
            from ABProcSystem.dbo.TASK_new t with (nolock)
            left join ABProcSystem.dbo.TASK_Q_new q with (nolock) on q.task_guid = t.task_guid
            where t.process_guid = (select t2.process_guid from ABProcSystem.dbo.TASK_new t2 with (nolock) where t2.task_guid = @taskTreeLeafGuid)
            """;
        var allTasks = await _connection.QueryAsync<Task1>(
            queryAllTasks,
            new { taskTreeLeafGuid = leafTask }
        );

        var taskTree = ConvertToTaskTree(allTasks);

        var node = taskTree[leafTask];
        var taskGuidsToClose = new List<Task1>();
        
        if(node?.Children?.Count(t => t.IsOpen) > 0)
            return taskGuidsToClose;
        
        while (node is not null && (node.Children is null || node.Children.Count(t => t.IsOpen) <= 1))
        {
            taskGuidsToClose.Add(node);
            node = node.Parent;
        }

        return taskGuidsToClose;
    }

    private Dictionary<Guid, Task1> ConvertToTaskTree(IEnumerable<Task1> tasks)
    {
        ArgumentNullException.ThrowIfNull(tasks);
        var dict = tasks.ToDictionary(task1 => task1.TaskGuid);

        foreach (var (_, task) in dict)
        {
            var parentGuid = task.IsRedirect ? null : task.ParentGuid;
            if (parentGuid is null || parentGuid == Guid.Empty)
                continue;
            if (!dict.ContainsKey(parentGuid.Value))
                throw new Exception("Parent not found");

            var parent = dict[parentGuid.Value];
            task.Parent = parent;
            parent.Children ??= new List<Task1>();
            parent.Children.Add(task);
        }

        return dict;
    }

    private async Task ForwardRequest(Guid requestGuid)
    {
        const string querySubrequestsCount = """
                select count(*) from ABProcSystem.dbo.TASK_Q_new with (nolock)
                where request_guid = @requestGuid and task_type like 'subrequest%'
                """;
        var subrequestsCount = await _connection.ExecuteScalarAsync<int>(
            querySubrequestsCount,
            new { requestGuid }
        );

        if (subrequestsCount == 0)
        {
            const string queryRequestExecutorCode = """
                    select EXECUTOR_CODE from ABProcSystem.dbo.MESSAGE_REQUEST with (nolock)
                    where REQUEST_GUID = @requestGuid
                    """;
            var requestExecutorCode = await _connection.ExecuteScalarAsync<string>(
                queryRequestExecutorCode,
                new { requestGuid }
            );
            await _processService.SendReplyMessageAsync(
                new SubrequestXmlModelBase { Reply = "accept" },
                requestGuid,
                requestExecutorCode
            );
        }
    }

    private async Task CloseTasks(IEnumerable<Guid> taskGuids)
    {
        if (_connection.State == ConnectionState.Closed)
            _connection.Open();

        using var transaction = _connection.BeginTransaction();
        try
        {
            const string executeDeleteTaskQueue =
                "delete from ABProcSystem.dbo.TASK_Q_new where task_guid in @taskGuidsToClose";
            var taskGuidsToClose = taskGuids.ToList();
            await _connection.ExecuteAsync(
                executeDeleteTaskQueue,
                new { taskGuidsToClose },
                transaction
            );
            const string executeUpdateTask = """
                update ABProcSystem.dbo.TASK_new
                set task_status = 'completed',
                    replied_by = SYSTEM_USER,
                    reply_date = getDate(),
                    reply_dec = 'accept',
                    reply_decn = N'Исполнено',
                    reply_comment = N'Исполнено',
                    approved_by = iif(approval_required = 1, SYSTEM_USER, approval_dec),
                    approval_date = iif(approval_required = 1, getDate(), approval_dec),
                    approval_dec = iif(approval_required = 1, 'approve', approval_dec),
                    approval_decn = iif(approval_required = 1, N'Согласовано', approval_dec),
                    approval_comment = iif(approval_required = 1, N'Согласовано', approval_dec)
                where task_guid in @taskGuidsToClose;
                """;
            await _connection.ExecuteAsync(
                executeUpdateTask,
                new { taskGuidsToClose },
                transaction
            );
            
            const string executeUpdateChildCount = """
                update ABProcSystem.dbo.TASK_new
                set child_count = iif(child_count = 0, 0, child_count - 1)
                where task_guid in (select parent_guid from ABProcSystem.dbo.TASK_new where task_guid in @taskGuidsToClose);
                update ABProcSystem.dbo.TASK_Q_new
                set child_count = t.child_count
                from ABProcSystem.dbo.TASK_Q_new tq
                inner join ABProcSystem.dbo.TASK_new t on t.task_guid = tq.task_guid
                where tq.task_guid in (select parent_guid from ABProcSystem.dbo.TASK_new where task_guid in @taskGuidsToClose);
                """;
            await _connection.ExecuteAsync(
                executeUpdateChildCount,
                new { taskGuidsToClose },
                transaction
            );
            
            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}
