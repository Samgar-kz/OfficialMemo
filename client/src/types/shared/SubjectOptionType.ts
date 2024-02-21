export type SubjectOptionType = {
  name: string;
  subcategories: {
    name: string;
    topics: {
      name: string;
    }[];
  }[];
};
