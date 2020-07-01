import ModalForm, { ActionType } from './ModalForm'


export interface ModalFormState {
    visible: boolean;
    title: string;
    params?: { [key: string]: any }
}

export {
    ActionType as ModalFormAction
}

export default ModalForm;