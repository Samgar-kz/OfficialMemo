import axios from "@/server/axios";

type AddDayOptions = {
  addRule?: "inclusive" | "exclusive";
  dayType?: "calendarDay" | "workingDay";
};
export default async function addDays(
  date: Date,
  days: number,
  options: AddDayOptions = {
    addRule: "inclusive",
    dayType: "calendarDay",
  }
): Promise<Date> {
  const { addRule, dayType } = options;

  const t = new Date(date);
  if (addRule == "inclusive") days = days - 1;

  if (dayType == "calendarDay") {
    t.setDate(t.getDate() + days);
    return t;
  } else {
    const { data } = await axios.calendarApiClient.get<Date>(
      `calendar/${t.toLocaleDateString("kk-KZ")}/add-working-days/${days}`
    );
    return new Date(data);
  }
}
