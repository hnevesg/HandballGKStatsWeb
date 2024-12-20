import { Route, Switch } from 'wouter';

import Register from './sections/register';
import LoginStaff from './sections/loginstaff';
import LoginPlayers from './sections/loginplayers'
import HomePage from './sections/home';
import Portal from "./sections/pre-login_portal"
import About from './sections/about';
import Streaming from './sections/streaming';

function App() {
    return (
    <>
      {/* App Routes */}
      <Switch>
        <Route path="/" component={Portal}/> 
        {/* Sign Up & Sign in */}
        <Route path="/register" component={Register} /> 
        <Route path="/loginplayers" component={LoginPlayers}/> 
        <Route path="/loginstaff" component={LoginStaff}/> 

        <Route path="/home" component={HomePage} />
       
        <Route path="/streaming" component={Streaming} />
        <Route path="/about" component={About} />

        {/* 404 Not Found page*/}
        <Route>
          <div>Page not found</div>
        </Route>
      </Switch>    </>
  );
};

export default App
