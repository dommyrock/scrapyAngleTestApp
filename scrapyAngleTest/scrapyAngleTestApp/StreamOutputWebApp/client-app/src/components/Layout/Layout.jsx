import React from 'react'
import { BrowserRouter, Switch, Route } from "react-router-dom";
import  ExpenseTracker  from "../ExpenseTracker/ExpenseTracker";
import  SignalRStream  from "../SignalrStream/Stream";

const Layout = () => {
    return (
        <BrowserRouter>
                <Switch>
                    <Route exact path="/expenses" component={ExpenseTracker} />
                    <Route exact path="/stream" component={SignalRStream} />
                </Switch>
        </BrowserRouter>
    )
}
export default  Layout;