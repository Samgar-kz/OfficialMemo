import axios from "@/server/axios";
import type { Employee } from "@/types/employees/Employee";

export default async function searchByName(
  searchString: string,
  isOnlyEmployee: boolean
): Promise<Employee[]> {
  const { data } = await axios.OfficialMemo.get<Employee[]>(
    "Employees/search",
    {
      params: {
        searchString,
        isOnlyEmployee,
      },
    }
  );
  return data;
}
