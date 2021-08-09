import React, {useState} from 'react'
import AtmForm from './AtmForm'
import { useForm } from '../../hooks/useForm';

const getFreshModelObject = () => ({
    cardNumber: '',
    withdrawAmount: 0,
    moneyPaperTypes: []
})

export default function ATMachine() {
    const {
        values,
        setValues,
        errors,
        setErrors,
        handleInputChange,
        resetFormControls
    } = useForm(getFreshModelObject);

    return (
        <AtmForm
            {...{
                values,
                setValues,
                errors,
                setErrors,
                handleInputChange,
                resetFormControls
            }}
        />
    )
}
