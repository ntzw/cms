import { FormItemType } from '@/components/DynamicForm';
import { ModalBase } from '@/components/ListTable';

export interface ContentFormProps {
    columnFields: ColumnField[];
    columnNum: string;
    itemNum?: string;
    actionRef?: React.MutableRefObject<ContentFormAction | undefined> | ((actionRef: ContentFormAction) => void);
    onFinish: (value: T) => Promise<any>;
}

export interface AsyncContentFormProps {
    columnNum: string;
    itemNum?: string;
    actionRef?: React.MutableRefObject<AsyncContentFormAction | undefined> | ((actionRef: AsyncContentFormAction) => void);
}

export interface ColumnField extends FieldDefaultType {
    columnNum: string;
}

export interface FieldDefaultType extends ModalBase {
    name: string;
    explain: string;
    optionType: FormItemType;
    options: string;
}

export interface ContentFormAction {
    submit: () => void;
    setValue: (value: any) => void;
    clear: () => void;
}

export interface AsyncContentFormAction {
    reload: () => void;
    submit: () => void;
}