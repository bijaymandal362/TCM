interface IProfileDetails {
  email: string;
  name: string;
  phoneNumber: string;
  role: string;
  roleId: number;
  userName: string;
  userProfileId: number;
  profileImage?: string; // just a placeholder for later & current api never have this field
}

export type ProfileDetails = IProfileDetails | null;

export interface UpdatePreferencesBody {
  personPersonalizationId: number;
  personId: number;
  themeListItemId: number;
  timeZone: string;
}

export interface Preferences {
  personPersonalizationId: number;
  personId: number;
  themeListItemId: number;
  themeType: string;
  timeZone: string;
}
