# README for React.js #

# Installation Steps #

1. Added React.NET nuget package into ITS.Retail.WebClient Project
2. Under Project's Script folder added react folder
3. Created new project with npm --init with the following packages dependencies. See package.json for dependencies. Highly inpired by the following [https://medium.com/@scbarrus/how-to-get-test-coverage-on-react-with-karma-babel-and-webpack-c9273d805063](https://medium.com/@scbarrus/how-to-get-test-coverage-on-react-with-karma-babel-and-webpack-c9273d805063 "guide")
4. Added karma.conf.js. Karma is used for testing setup and automation
5. Added webpack.config.js. More info at the following [http://reactjs.net/guides/webpack.html](http://reactjs.net/guides/webpack.html "link"). Webpack module bundler includes eslint and jscs linters, and is used to build components to Scripts/build folder
6. Changed the folder structure to have a root folder src before components
7. Components folder is the location where all jsx files reside.
8. Added **extensions** folder for other js files which are not react components.
9. Added mocha, chai, karma, babel to name a few.

# React Components  #

## Creation ##

1. To add new react component just create in on component folder under Scripts/react/src/
2. Additionally edit index.js under the same folder and add the new component
3. To compile all jsx files you need to open command line cd to react folder and just execute **webpack --progress --colors --watch or npm run:devpack**
4. The previous command creates 2 files under Scripts/build client.js & server.js that are needed for the React.net both for the server side and client side. More info at the following [http://reactjs.net/guides/webpack.html](http://reactjs.net/guides/webpack.html "link")

## Linting ##

1. Added eslint, jsscs and stylelint for js and css files with their respective configs .eslintrc, .jscsrc
2. Linting is automatically executed upon build among with other scripts with command npm run dev:webpack

## Testing ##

1. Along with the creation of a component we create a __test__ folder with the same name as our component followed by a dash and test.
3. Our test uses mocha testing framework,chai for assertion and mocking

## Styling ##

1. Added Postcss. More info [https://github.com/postcss/postcss https://css-tricks.com/bem-101/](https://github.com/postcss/postcss https://css-tricks.com/bem-101/ "More")
2. Added gulpfile.js to compile css files into Scripts/build folder for react components using postcss
3. Used suitcss naming convention. (https://github.com/suitcss/suit/blob/master/doc/STYLE.md)
4. Using Lost Grid as a grid system
5. Added various postcss plugins (see package.json for more)

## Documentation ##

1. Added Gruntfile.js for generating documentation.
2. Use jsdoc syntax
3. Run **npm run dev:doc** to generate documentation

## UI Testing ##

UI Testing with with Nightwatch.js and Mocha.Supporting e2e testing with selenium. [http://nightwatchjs.org/guide#using-mocha ](http://nightwatchjs.org/guide#using-mocha  "More")

1. Changed webpack configuration to include nightwatch.js
2. Added selenium-standalone in package.json
3. Created folder e2e where all e2e tests reside
4. Added folder e2e/reports where reports are exported.

## Usefull Commands ##

Open a cmd on react folder and execute on of the following commands:

1. npm **run dev:webpack** - Watches for file changes in  jsx files and builds the scripts under the folder /Scripts/src/components/
2. **npm test** - Executes karma start
3. **npm run e2e** - Executes nightwatch tests. *If you see an error it *
4. **npm run dev:gulp** - Run css compiler babel compiler and various gulp tasks
5. **npm run dev:doc** - Run docs generations with grunt
6. **npm run clean** - Clean build folder **WARNING - FILE DELETION**
7. **npm run build** - Run all above tasks in parallel using npm-run-all. *If you receive an error while executing this command you probably haven't install npm-run-all package. Install it by executing
8. **npm run production** - Run tasks for production

## Razor Views - Bundle Config ##

  1. Added the necessary scripts and css inside BundleConfig

  2. Inside our razor.html we can use the following examples

	1. `@Html.React("Components.CommentsBox", new { initialComments = Model.Comments })` *Expect object *
	2. `@Html.React("Components.HelloWorld", new { name = "Daniel" })` *Expect string*

## Best Practices ##

1. Use Babel ES6-7 syntax when possible
2. Always create and run tests

## Updating ##

Install npm-install-updates and run ncu.

## React Plugins ##

- [https://github.com/AllenFang/react-bootstrap-table ](https://github.com/AllenFang/react-bootstrap-table )
- [http://www.gpbl.org/react-day-picker/docs/GettingStarted.html ](http://www.gpbl.org/react-day-picker/docs/GettingStarted.html )
- [https://github.com/GriddleGriddle/Griddle](https://github.com/GriddleGriddle/Griddle)

## TODO ##
- Add cssnext in gulp procedure
- Add sinon for stubbing ajax calls
- Add flow for static analysis. On 7/3/2016 is got and **Error** on Windows Check for another version in the future


## Usefull Links ##

1. [Test Utils](https://facebook.github.io/react/docs/test-utils.html)
2. [Babel Syntax](https://babeljs.io/docs/learn-es2015/ )
3. [React Testing](https://github.com/robertknight/react-testing)
4. [Webpack + React Book](http://survivejs.com/webpack_react/introduction/)
5. React Styling [Link 1](http://survivejs.com/webpack_react/styling_react/) - [Link 2](https://github.com/css-modules/webpack-demo)
6. PostCSS [Link 1](https://github.com/postcss/postcss#what-is-postcss) - [Link2](http://articles.dappergentlemen.com/2015/07/24/postcss/) - [Link3](http://postcss.parts/)
7. [Chai](http://chaijs.com/guide/styles/#expect)
8. [Suitcss](https://github.com/suitcss/suit)
9. [Babel Update](https://medium.com/@malyw/how-to-update-babel-5-x-6-x-d828c230ec53)
10. ES6 for React [Link 1](http://www.newmediacampaigns.com/blog/refactoring-react-components-to-es6-classes) -  [Link 2](http://ilikekillnerds.com/2015/02/developing-react-js-components-using-es6/)
11. [ES6+ for React](http://babeljs.io/blog/2015/06/07/react-on-es6-plus/)
12. [Flux](https://facebook.github.io/flux/docs/todo-list.html#content)
13. [React Notification System](https://github.com/igorprado/react-notification-system)
14. [React Bootstrap](http://react-bootstrap.github.io/components.html#utilities)
15. [React Grid](https://github.com/GriddleGriddle/Griddle)
16. [React Form](https://github.com/insin/newforms)
17. [React Flux Form](https://github.com/erikras/redux-form)
18. [React Example .NET MVC](https://github.com/csoulioti/ToDoMVC)
19. [Enzyme - Test Utility for React](https://github.com/airbnb/enzyme)
20. [RxJS](https://github.com/Reactive-Extensions/RxJS)
21. [RxJS with React](http://www.codeproject.com/Articles/1060081/Reactive-Autonomous-States)
22. [Redux with React](http://rackt.org/redux/docs/basics/UsageWithReact.html)
23. [Redux Lessons](https://egghead.io/series/getting-started-with-redux)
24. [Npm Update](https://www.npmjs.com/package/npm-check-updates)
25. [React Widgets](http://jquense.github.io/react-widgets/docs/#/multiselect)
26. [Airbnb Style Guide](https://github.com/airbnb/javascript)
27. [Higher Order React Component](http://egorsmirnov.me/2015/09/30/react-and-es6-part4.html)
28. [ES2015](https://leanpub.com/ecmascript2015es6guide/read#leanpub-auto-strings)
29. [React Cheatsheet](http://reactcheatsheet.com/)
30. [Sinon, Mocha, Chai](https://sean.is/writing/client-side-testing-with-mocha-and-karma/)
31. [React Intl](http://formatjs.io/react/#formatted-message)
32. [ReactJS.NET Webpack Demo](https://github.com/reactjs/React.NET/tree/master/src/React.Sample.Webpack)
33. [wallabyjs for testing](http://wallabyjs.com/docs/integration/overview.html)
34. [ES5 to ES6](http://cheng.logdown.com/posts/2015/09/29/converting-es5-react-to-es6)
35. [React and Flux](https://medium.com/@tribou/react-and-flux-migrating-to-es6-with-babel-and-eslint-6390cf4fd878#.mijybl19h)
36. [Lost Columns Grid System](https://github.com/peterramsing/lost#lost-column)
37. [ECMASCRIPT2016 Guide](https://leanpub.com/ecmascript2015es6guide/read#leanpub-auto-strings)
38. [React Cheatsheet](http://reactcheatsheet.com/)
39. [React Validation Mixin](https://github.com/jurassix/react-validation-mixin)
40. [Isolated Unit Tests with Sinonjs](https://kostasbariotis.com/isolated-unit-tests-with-sinonjs/)
41. [**React Makes You Sad**](https://github.com/gaearon/react-makes-you-sad)
42. [**React How To**](https://github.com/petehunt/react-howto)
43. [Eradicate Runtime Errors in React with Flow](http://technologyadvice.github.io/eradicate-runtime-errors-in-react-with-flow/)
44. [Step by Step Guide To Building React Redux Apps](https://medium.com/@rajaraodv/step-by-step-guide-to-building-react-redux-apps-using-mocks-48ca0f47f9a#.vksc2066w)
45. [A Guide For Building A React Redux CRUD App](https://medium.com/@rajaraodv/a-guide-for-building-a-react-redux-crud-app-7fe0b8943d0f#.v0bex43y8)
46. [Example React Test Code with assert](https://github.com/survivejs/webpack_react/blob/master/project_source/09_testing_react/kanban_app/tests/editable_test.js)
47. [Bluebird Promise](http://bluebirdjs.com/docs/working-with-callbacks.html)
48. [FlowType](http://flowtype.org/)
49. [Building Redux Middleware](https://reactjsnews.com/redux-middleware)
50. [Redux Documentation](http://redux.js.org/index.html)
51. [Redux Forms](http://redux-form.com/4.2.0/#/examples/dynamic?_k=edqb5b)
52. [React Router Basics](http://adambac.com/what-practical-programmer-should-know-about-react-router/)
53. [React Templates](http://wix.github.io/react-templates/)
54. [Enzyme](https://github.com/airbnb/enzyme)
55. [React-Motion](https://github.com/chenglou/react-motion)
