import { Provider } from "react-redux";
import { BrowserRouter } from "react-router-dom";
import { ErrorBoundary } from "./hoc/ErrorBoundary";
import Routes from "./routes";
import { store } from "./store/store";

function App() {
  return (
    <BrowserRouter>
      <ErrorBoundary>
        <Provider store={store}>
          <Routes />
        </Provider>
      </ErrorBoundary>
    </BrowserRouter>
  );
}

export default App;
