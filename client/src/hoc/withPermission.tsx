import React from "react";

const withPermission = (slug:any, Component: React.FC<any>) => {
  return (props: any) => {

    if (slug) {
      if (props.route.permission.includes(slug)) {
        return <Component {...props} />;
      } else {
        return (
          <div className="error-wrap">
            <h1 className="error-head">404</h1>
            <p>Page not found</p>
            {/* <Button type="primary">Back</Button> */}
          </div>
        );
      }
    }
    return null;
  };
};

export default withPermission;
