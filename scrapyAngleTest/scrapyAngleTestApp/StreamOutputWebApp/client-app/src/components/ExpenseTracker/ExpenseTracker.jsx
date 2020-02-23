import React from 'react'
import { Balance } from "../ExpenseTracker/Balance";
import { IncomeExpenses } from "../ExpenseTracker/IncomeExpenses";
import { TransactionList } from "../ExpenseTracker/TransactionsList";
import { AddTransaction } from "../ExpenseTracker/AddTransaction";

 import "./expenses.css"



const inlineCss = {
    textAlign: 'center'
  }
 const ExpenseTracker = () => {
    return (
        <>
            <h2 style={inlineCss}>
                Expense tracker
            </h2>
            <hr />
            <div className="container">
                <Balance />
                <IncomeExpenses />
                <TransactionList />
                <AddTransaction />

            </div>
        </>
    )
}
export default ExpenseTracker;