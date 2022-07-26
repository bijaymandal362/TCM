export const handleError = (err : any, setServerError: any, setLoading: any) => {
  if(err.response.data.succeeded){
   setServerError(err.response.data.data.errors[0]);
 } else {
   setServerError(err.response.data.message)
 }
 setLoading(false);
}

export const handleSuccess = (err: any, setSuccessHandle: any, setLoading: any) => {
 
}