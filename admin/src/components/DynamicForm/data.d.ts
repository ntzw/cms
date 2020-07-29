import { FormInstance, RuleObject, FormProps } from 'antd/lib/form';
import { InputProps, TextAreaProps, PasswordProps } from 'antd/lib/input';
import { SelectProps } from 'antd/lib/select';
import { CascaderProps, CascaderOptionType } from 'antd/lib/cascader';
import { SwitchProps } from 'antd/lib/switch';
import { Store } from 'antd/lib/form/interface';

export interface FormItem {
    label: string;
    name: string;
    extra?: string;
    type?: FormItemType;
    dataAction?: string;
    valuePropName?: string;
    validateTrigger?: string;
    split?: string;
    rules?: RuleObject[];
    input?: InputProps;
    password?: PasswordProps;
    textarea?: TextAreaProps;
    select?: SelectProps<any>;
    cascader?: CascaderProps;
    switch?: SwitchProps;
}

export interface DynaminFormProps<T extends Store> {
    fields: FormItem[] | string;
    params?: { [key: string]: any };
    name?: string;
    onFinish?: (value: T) => Promise<any>;
    /**
  * 初始化的参数，可以操作 Form
  */
    actionRef?: React.MutableRefObject<DynaminFormAction | undefined> | ((actionRef: DynaminFormAction) => void);
    fieldActionParams?: (field: FormItem) => ({ [key: string]: any } | null);
    layout?: {
        labelCol?: FormProps['labelCol'];
        wrapperCol?: FormProps['labelCol'];
    }
}

export interface DynaminFormAction {
    reload: () => void;
    clear: () => void;
    reloadFieldItem: () => void;
    submit: () => void;
    setValue: (value: any) => void;
    getValue: () => any;
    setLoading: (loading: boolean) => void;
    setOldValue: () => void;
}

interface AsyncResult<T extends Store> {
    editData: T,
    field: FormItem[];
}