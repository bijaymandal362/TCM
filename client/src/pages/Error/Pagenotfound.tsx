import { ArrowLeftOutlined } from "@ant-design/icons";
import { Button } from "antd";
import { useHistory } from "react-router-dom";
import PageNotFound from "../../assets/images/error-404.png";

function Pagenotfound() {
  let history = useHistory();

  return (
    <div className="error__page__wrapper">
      <div className="page__inside">
        <div>
          <h1>Page not found</h1>
          <p>
            We've noticed you lost your way, no worries, we will help you to
            found the correct path.
          </p>
          <Button className="link__goback" onClick={history.goBack}>
            <ArrowLeftOutlined /> Go back
          </Button>
        </div>
        <img src={PageNotFound} alt="" />
      </div>
    </div>
  );
}

export default Pagenotfound;
