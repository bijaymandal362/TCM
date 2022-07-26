import { mainServerApi } from "../mainServer";
import { ProfileDetails } from "../../../interfaces/profileDetails.interface";
import { addAuthHeader } from "../../../util/auth.util";

const profileApi = mainServerApi.injectEndpoints({
  endpoints: (build) => ({
    getUserProfile: build.query<ProfileDetails, void>({
      query: () => ({
        url: "UserProfile/GetUserProfileDetails",
        headers: {
          ...addAuthHeader(),
        },
      }),
    }),
  }),
  overrideExisting: false,
});

export const { useGetUserProfileQuery } = profileApi;
