import axios from "@/server/axios";
import type { Employee } from "@/types/employees/Employee";

export default async function whoAmI(): Promise<Employee> {
  const { data } = await axios.OfficialMemo.get<Employee>("Employees/whoAmI");
  return data;
}
