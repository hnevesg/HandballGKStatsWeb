import { Route, Switch } from 'wouter';

import Register from './sections/register';
import LoginStaff from './sections/loginstaff';
import LoginPlayers from './sections/loginplayers'
import HomePage from './sections/home';
import Portal from "./sections/pre-login_portal"
import About from './sections/about';
import Streaming from './sections/streaming';
import PlayerSection from './sections/Player Section/playerSection';
import PlayersComparison from './sections/Comparison Section/playersComparison';
import PlayerSessions from './sections/Player Section/playerSessions';
import SessionDetails from './sections/Player Section/sessionDetails';
import PlayerProgress from './sections/Player Section/playerProgress';
import PlayersSessions from './sections/Comparison Section/playersSessions';
import ComparisonDetails from './sections/Comparison Section/comparisonDetails';

function App() {
  return (
    <>
      {/* App Routes */}
      <Switch>
        <Route path="/" component={Portal} />
        {/* Sign Up & Sign in */}
        <Route path="/register" component={Register} />
        <Route path="/loginplayers" component={LoginPlayers} />
        <Route path="/loginstaff" component={LoginStaff} />

        <Route path="/home" component={HomePage} />

        {/* Personal Section */}
        <Route path="/player-section" component={PlayerSection} />
        <Route path="/player-sessions" component={PlayerSessions} />
        <Route path="/statistics-details" component={SessionDetails} />
        <Route path="/player-progress" component={PlayerProgress} />

        {/* Comparisons Section */}
        <Route path="/players-comparison" component={PlayersComparison} />
        <Route path="/players-sessions" component={PlayersSessions} />
        <Route path="/comparison-details" component={ComparisonDetails} />

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
