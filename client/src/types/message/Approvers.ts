import type { Executor } from '@/types/message/Executor';

interface Approvers {
    approver: Executor[] | undefined;
    approverList?: string;
    approverCount?: string;
    agreeCount?: string;
}

export type { Approvers }
