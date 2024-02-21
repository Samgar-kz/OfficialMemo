export type Client = {
  key: string;
  id: string;
  identityNumber?: string; // ИИН/БИН
  name: string;
  email?: string;
  phone?: string;
  address?: string;
  hasEsedo?: boolean;
  clientType?: ClientType;
  member?: string;
};

export type EntityClient = Client & {
  email2?: string;
  author?: string;
};

export type ClientType = "physic" | "entity";
