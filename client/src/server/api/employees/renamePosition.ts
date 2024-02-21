import axios from "@/server/axios";
import type { Employee } from "@/types/employees/Employee";

export default async function renamePosition(renameRequest: Employee) {
  await axios.OfficialMemo.post("employees/position/rename", renameRequest);
}
