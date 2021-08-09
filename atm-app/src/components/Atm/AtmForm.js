import { Grid } from '@material-ui/core'
import React, { useState } from 'react'
import Form from '../../layouts/Form'
import { Button, Input, Select } from '../../controls'
import { createAPIEndpoint, OPERATIONS } from "../../api";
import Notification from "../../layouts/Notification";
import eur50 from "../../images/50.jpg";//<img src={eur50} />
import eur20 from "../../images/20.jpg";
import eur10 from "../../images/10.jpg";
import eur5 from "../../images/5.jpg";
//import { Alert } from 'react-alert'

export default function AtmForm(props) {

    const { values, setValues, errors, setErrors, handleInputChange, resetFormControls } = props;
    const [cardInserted, setCardInserted] = useState(false);
    const [notify, setNotify] = useState({ isOpen: false })
    //const classes = useStyles();

    const [cardNumber, setCardNumber] = useState();

    const validateCardNumber = () => {
        let temp = {};
        temp.cardNumber = values.cardNumber != '' ? "" : "This field is required.";
        //temp.withdrawAmount = values.withdrawAmount != '' ? "" : "This field is required.";
        setErrors({ ...temp });
        return Object.values(temp).every(x => x === "");
    }
    const validateWithdrawAmount = () => {
        let temp = {};
        temp.withdrawAmount = values.withdrawAmount > 0 && !isNaN(values.withdrawAmount) ? "" : "This field should be a number and more than zero.";
        setErrors({ ...temp });
        return Object.values(temp).every(x => x === "");
    }

    const insertCard = () => {
        if (validateCardNumber()) {
            //alert(values.cardNumber);
            createAPIEndpoint(OPERATIONS.INSERT_CARD).insertCard(values.cardNumber)
                .then(res => {
                    setNotify({ isOpen: true, message: res.data });
                    //alert(res.data);
                    console.log(res);
                    setCardInserted(true);
                })
                .catch(err => console.log(err))
        }
    }

    const returnCard = () => {
        createAPIEndpoint(OPERATIONS.RETURN_CARD).returnCard()
            .then(res => {
                setNotify({ isOpen: true, message: res.data });
                //alert(res.data);
                console.log(res);
                setCardInserted(false);
                resetFormControls();
            })
            .catch(err => console.log(err))
    }

    const getSupportedPaperNoteTypes = () => {
        createAPIEndpoint(OPERATIONS.GET_SUPPORTED_PAPER_NOTES).getSupportedPaperNoteTypes()
            .then(res => {
                setNotify({ isOpen: true, message: res.data });
                //alert(res.data);
                console.log(res);
            })
            .catch(err => console.log(err))
    }

    const getCardBalance = () => {
        createAPIEndpoint(OPERATIONS.GET_CARD_BALANCE).getCardBalance()
            .then(res => {
                setNotify({ isOpen: true, message: "Your balance is " + res.data + "€" });
                //alert(res.data);
                console.log(res);
            })
            .catch(err => console.log(err))
    }

    const withdrawMoney = () => {
        if (validateWithdrawAmount()) {
            createAPIEndpoint(OPERATIONS.WITHDRAW_MONEY).withdrawMoney(values.withdrawAmount)
                .then(res => {
                    //alert(res.data.Amount + "€ successfully withdrawn from your account.");
                    console.log(res);
                    if (res != null && res.data != null && res.data.Amount > 0) {
                        if (res.data.Notes != null) {
                            var str = '';
                            if (res.data.Notes.Fifty > 0)
                                str = "\n [50€ x " + res.data.Notes.Fifty + "]";
                            if (res.data.Notes.Twenty > 0)
                                str += "\n [20€ x " + res.data.Notes.Twenty + "]";
                            if (res.data.Notes.Ten > 0)
                                str += "\n [10€ x " + res.data.Notes.Ten + "]";
                            if (res.data.Notes.Five > 0)
                                str += "\n [5€ x " + res.data.Notes.Five + "]";
                        }
                        setNotify({ isOpen: true, message: res.data.Amount + "€ successfully withdrawn from your account." + str });
                    }
                    if (res != null && res.data != null && res.data.Message != null) {
                        //Server side exception
                        setNotify({ isOpen: true, message: res.data.Message });
                        let temp = {};
                        temp.withdrawAmount = res.data.Message;
                        setErrors({ ...temp });
                    }
                })
                .catch(err => {
                    console.log(err);
                    //throw new Error(err);
                })
        }
    }

    return (
        <>
            <Form>
                <Grid container>
                    <Grid item xs={6}>
                        <Input
                            label="Card Number"
                            name="cardNumber"
                            disabled={cardInserted}
                            value={values.cardNumber}
                            onChange={handleInputChange}
                            error={errors.cardNumber}
                        />
                        <Button
                            size="large"
                            onClick={insertCard}
                            name="insertCardBtn"
                            disabled={cardInserted}
                        >Insert Card</Button>
                        <Button
                            size="large"
                            onClick={returnCard}
                            name="returnCardBtn"
                            disabled={!cardInserted}
                        >Return Card</Button>
                        <Button
                            size="large"
                            onClick={getCardBalance}
                            name="getCardBalanceBtn"
                            disabled={!cardInserted}
                        >Get Card Balance</Button>
                        
                    </Grid>
                    <Grid item xs={6}>
                            <Input
                                label="Withdraw Amount"
                                name="withdrawAmount"
                                disabled={!cardInserted}
                                value={values.withdrawAmount}
                                onChange={handleInputChange}
                                error={errors.withdrawAmount}
                            />

                            <Button
                                size="large"
                                onClick={withdrawMoney}
                                name="withdrawMoneyBtn"
                                disabled={!cardInserted}
                            >Withdraw Money</Button>
                    </Grid>
                </Grid>
            </Form>
            <Notification
                {...{ notify, setNotify }} />
        </>
    )
}
