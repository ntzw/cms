import ModalForm, { ModalFormAction } from './ModalForm'

interface ModalFormState {
    visible: boolean;
    title: string;
    isUpdate: boolean;
    params?: { [key: string]: any };   
}

export type {
    ModalFormState,
    ModalFormAction
}

export default ModalForm;