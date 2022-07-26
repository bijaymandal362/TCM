# Document for Dashboard theme

## The initial flow uses following packages.

- Reactjs
- Redux
- Ant design
- React-router-dom
- Axios
- Node-sass

# Folder Structure of src

1.  assets = This folder contains all the static assets such as sass, css, images.

2.  axios = This folder contains the axios instance with all the initial config. This is the place that tracks all the incoming and outgoing api request from the frontend app. So, one can handle refresh token or any other task to be done for particular status code.

3.  components = All the components that is shared between main components is constructed here.

4.  enum = all the enums used in the app is defined in this folder.

5.  hoc = Higher Order Components, this folder contains hoc that wraps child component to pass its functionality.

6.  interfaces = this folder contains the type for variables, modules, components, etc.

7.  pages = this folder contains ta main routing component or containers.

8.  routes = Routes folder has two kind of routing file

- auth.routes.ts = This folder contains all the routes related to auth and can only be accessible if the user is not logged in. Those routes don’t contain header and footer.
  Eg= /login , /register, forget-password, etc
- master.routes.ts = This folder contains all the routes that are protected and can only be accessible if the user is logged in. those routes contain header and footer.
  Eg= /dashboard, /reports, etc.

9. store = This folder contains redux store and its configuration this folder has subfolder

- Reducers = this sub folder contains all the redux state that is to be modified throughout the app interaction

- Action = This sub folder contains folder for creating custom action types and folder for creating custom action creators that contain asyc login such as api call to dispatch some actions

10. util = This folder contains utility or helper functions that is required from the different parts of the program.

## Usage of this code base

# Creating a new page

- Decide if the page is protected or not.
  If the page is protected add the routes to master.route.tsx file inside route folder
  ` src/route/master.route.tsx`
  else add routes to auth.route.tsx

  Routes is to added in following manners

  Routes interface

```ts
export interface IRouteItem extends RouteProps {
  name: string;
  path: string;
  exact?: boolean;
  LazyComponent?: any;
  icons?: React.ReactNode;
}
```

Example

```ts
export const mainRoutesList: IRouteItem[] = [
  {
    name: "Dashboard",
    path: "/",
    LazyComponent: lazy(() => import("../pages/Home/Home")),
    exact: true,
  },
  {
    name: "Form",
    path: "/form",
    LazyComponent: lazy(() => import("../pages/Form/Form")),
    exact: true,
  },
];
```

To add new routes add a object with the following properties to yourselected.route.tsx file.
Name = Name of route
Path = what url should load that page
LazyComponent = import your page as shown in above picture for code splitting.
Exact = to load the page if the url is a exact match
Icons= if the page requires any icon that can be added in the object as defines in the router interface IrouteItem.

If any new property is to be added in the routes than add it first on the interface and then make changes in routes file.

All this property can be accessible to the pages by its props.

# Creating a new link

Any new links to be created for loading new page should be kept in sideNavList.tsx file inside component folder.

```ts
export interface NavLinks {
  href: string;
  icon: React.ReactNode;
  title: string;
  children?: Array<NavLinks>;
}
```

Interface for links in side navigation

```ts
export const sideNavList: NavLinks[] = [
  { href: "/", icon: <DashboardOutlined />, title: "Default" },
  { href: "/analytics", icon: <DotChartOutlined />, title: "Analytics" },
  { href: "/form", icon: <DashboardOutlined />, title: "Form" },
  { href: "/fund", icon: <FundOutlined />, title: "Funds" },
  {
    href: "/report",
    icon: <DashboardOutlined />,
    title: "Reports",
    children: [
      { href: "/report/finance", icon: <DotChartOutlined />, title: "Finance" },
      { href: "/report/usage", icon: <DotChartOutlined />, title: "Usages" },
    ],
  },
];
```

Links is to be inserted in sideNavList.tsx file inside sideNavList array as shown in above diagram. Demonstration is shown for all links and links with sub category in the above example.

# Making an api and using Redux

Redux is used for global state management. The redux configuration is present in store folder.
`src/store`

- store.tsx = This file creates the redux store with all configuration and exports the store along with its type for typescript usage.

- ReduxHooks.txs = This file exports the rudux hooks i.e useDispatch as useAppDispatch and useSelector as useAppSelector so don’t use useDispatch and useSelector importing them from react-redux inside your component…. Because you will not receive the power of typescript since they will not have the types of your store.
  Instead use useAppDispatch and useAppSelector as their alternatives exported from reduxHooks.tsx inside ` src/store/reduxHooks.tsx`. Those files are exported with types.

Usage:

yourComponent.tsx

```ts
import { useAppSelector } from ‘path_to_reduxHooks.tsx’;

export function Abc() {
    const state = useAppSelector(state => state.your_reducer)
    return …..
}
```

Now for creating a reducer just create your_reducer.ts file inside reducers folder
`src/store/reducer/your_reducer.ts`

For creating action types create your_actiontypes.ts file inside actions folder and for creating action creators that handle api calls and async logics create your_actioncreator.ts
file inside actioncreators folder.

` src/store/actions/action_name/your_actiontypes.ts`
` src/store/actions/action_name/your_actiontypes.ts`

all api calls is to be make from actioncreators file for best practice.

# Sass usage for dark and light theme

The scss for colors should only be written in `\_lightvariable.scss` or `\_darkvariable.scss` for light theme and dark theme respectively.

1. The css of new pages should be written in scss partials in the page folder inside scss.
2. The css of components like buttons, tables, cards and so on should be written in scss partials in the component folder inside scss.
3. The scss partials’ name must begin with an “\_” and must be imported in the main scss file.
4. You can find the path of the scss folder inside the assets folder in the main src folder.
