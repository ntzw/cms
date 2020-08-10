import { FormItemType } from '@/components/DynamicForm';
import { ModalBase } from '@/components/ListTable';

interface ContentFormBaseProps {
    columnNum: string;
    itemNum?: string;
    isCategory?: boolean;
    isSeo?: boolean;
    isAllowTop?: boolean;
}

export interface ContentFormProps extends ContentFormBaseProps {
    columnFields: ColumnField[];
    actionRef?: React.MutableRefObject<ContentFormAction | undefined> | ((actionRef: ContentFormAction) => void);
    onFinish: (value: T) => Promise<any>;
}

export interface AsyncContentFormProps extends ContentFormBaseProps {
    actionRef?: React.MutableRefObject<AsyncContentFormAction | undefined> | ((actionRef: AsyncContentFormAction) => void);
}

export interface ColumnField extends FieldDefaultType {
    columnNum: string;
    sort: number;
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
    loading: (status: boolean) => void;
    reoladFieldItem: () => void;
}

export interface AsyncContentFormAction {
    reload: () => void;
    submit: () => void;
}

export interface SelectOptions {
    multiple: boolean;
}