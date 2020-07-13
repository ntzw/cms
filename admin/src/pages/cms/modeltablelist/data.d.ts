import { ModalBase } from '@/components/ListTable';
import { FieldDefaultType } from '../columnlist/data'

export interface ModelTableListProps {

}

export interface ModelTable extends ModalBase {
    tableName: string;
    explain: string;
}

export interface ModelFieldListProps extends ModelFieldListPropsState {
    onClose: () => void;
}

export interface ModelFieldListPropsState {
    modelItem?: ModelTable;
    visible: boolean;
}

export interface ModelField extends FieldDefaultType {
    modelNum: string;
}