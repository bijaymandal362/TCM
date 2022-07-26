import { mainServerApi } from "../mainServer";
import { addAuthHeader } from "../../../util/auth.util";
import {
  Preferences,
  UpdatePreferencesBody,
} from "../../../interfaces/profileDetails.interface";

const preferencesApi = mainServerApi.injectEndpoints({
  endpoints: (build) => ({
    getUserPreferences: build.query<Preferences, void>({
      query: () => ({
        url: "UserProfile/GetPersonPersonalizationByCurrentPersonId",
        headers: {
          ...addAuthHeader(),
        },
      }),
      providesTags: (result) => [
        { type: "Preferences", id: result?.themeListItemId },
        "Preferences",
      ],
    }),
    updateUserPreferences: build.mutation<any, UpdatePreferencesBody>({
      query: (body) => ({
        url: "UserProfile/UpdatePersonPersonalization",
        headers: {
          ...addAuthHeader(),
        },
        method: "PUT",
        body,
        responseHandler: (response) => response.text(),
      }),
      invalidatesTags: (result, error, args) => [
        { type: "Preferences", id: args.themeListItemId },
      ],
    }),
  }),
  overrideExisting: false,
});

export const { useGetUserPreferencesQuery, useUpdateUserPreferencesMutation } =
  preferencesApi;
