import axios from "axios";

const baseURL = process.env.REACT_APP_BASE_URL;

const axiosInstance = axios.create({
  baseURL: baseURL,
  headers: {
    Accept: "application/json",
  },
});

axiosInstance.interceptors.response.use(
  (response: any) => {
    return response;
  },
  (err) => {
    const originalRequest = err.config;
    if (err.message === "Network Error") {
      return new Error("Network Error");
    }

    if (err.response.status === 306 && !originalRequest._retry) {
      return axiosInstance;
    }
    //Handle refresh token here

    // let refreshToken = localStorage.getItem("refreshToken");

    // if (refreshToken && err.response.status === 306 && !originalRequest._retry) {
    //   originalRequest._retry = true;
    //   //do some stuff here........
    //   return axiosInstance({
    //       method:"POST",
    //       url:"",
    //       data:{
    //         refreshToken
    //       }
    //   }).then(res=>{
    //     if(res.status===200){
    //       localStorage.setItem("accessToken", res.data.accessToken);
    //                   console.log("Access token refreshed!");
    //      return axios(originalRequest);
    //     }
    //   });

    // }

    return Promise.reject({
      ...err,
      response: err.response,
      message: err?.message,
      status: err.response.status,
    });
  }
);

export default axiosInstance;
