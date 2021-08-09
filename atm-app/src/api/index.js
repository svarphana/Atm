import axios from "axios";

const BASE_URL = 'http://localhost:56166/api/atm/';

export const OPERATIONS = {
    INSERT_CARD: 'insertCard',
    RETURN_CARD: 'returnCard',
    GET_CARD_BALANCE: 'getCardBalance',
    GET_TTOTAL_MONEY: 'getSupportedPaperNoteTypes',
    GET_SUPPORTED_PAPER_NOTES: 'getSupportedPaperNoteTypes',
    WITHDRAW_MONEY: 'withdrawMoney'
}

export const createAPIEndpoint = endpoint => {

    let url = BASE_URL + endpoint + '/';
    return {
        insertCard: cardNumber => axios.get(url + cardNumber),
        returnCard: () => axios.get(url),
        getCardBalance: () => axios.get(url),
        getTotalMoney: () => axios.get(url),
        getSupportedPaperNoteTypes: () => axios.get(url),
        withdrawMoney: amount => axios.get(url + amount)
        /*fetchAll: () => axios.get(url),
        fetchById: id => axios.get(url + id),
        create: newRecord => axios.post(url, newRecord),
        update: (id, updatedRecord) => axios.put(url + id, updatedRecord),
        delete: id => axios.delete(url + id)*/
    }
}