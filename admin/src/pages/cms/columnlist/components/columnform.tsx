import React, { useEffect, useState } from 'react';
import { ColumnFormProps, ColumnFormAction } from '../data';
import DynaminForm, { FormItemType } from '@/components/DynamicForm';
import { DynaminFormProps, FormItem } from '@/components/DynamicForm/data';
import { GetColumnContentEdit } from '../service';
import { message } from 'antd';

interface BaseOptions {
    required?: boolean;
    regularTypes?: RegularType[];
}

interface InputOptions extends BaseOptions {
    maxLength?: number;
    addonBefore?: string;
    addonAfter?: string;
    extra?: string;
}

enum RegularType {
    MobilePhone = 1,
}

const ColumnForm: React.FC<ColumnFormProps> = ({ columnNum, itemNum, actionRef }) => {
    const [formFields, setFormFields] = useState<FormItem[]>([]);

    const action: ColumnFormAction = {
        reload: () => {
            GetColumnContentEdit(itemNum, columnNum).then(res => {
                if (res.isSuccess && res.data) {
                    const { fields } = res.data;

                    setFormFields(fields.map((temp): FormItem => {
                        const options: BaseOptions = JSON.parse(temp.options);
                        const { required, regularTypes, ...elseOptions } = options;

                        const item: FormItem = {
                            label: temp.explain,
                            name: temp.name,
                            type: temp.optionType,
                            rules: [],
                        };

                        if (required) {
                            item.rules?.push({
                                required: true,
                                message: `${temp.explain} 为必填项`
                            })
                        }

                        SetRegulars(regularTypes, item);
                        switch (item.type) {
                            case FormItemType.input:
                                SetInputOptions(item, elseOptions);
                                break;

                            default:
                                break;
                        }

                        return item;
                    }));
                } else {
                    message.error(res.message || '获取表单数据失败');
                }
            })
        }
    }

    if (actionRef && typeof actionRef === 'function') {
        actionRef(action);
    } else if (actionRef && typeof actionRef !== 'function') {
        actionRef.current = action;
    }

    useEffect(() => {
        action.reload();
    }, [columnNum, itemNum])

    const formProps: DynaminFormProps<any> = {
        fields: formFields,
        onFinish: () => {
            return new Promise(() => {

            })
        }
    }

    return <DynaminForm {...formProps} />
}

export default ColumnForm;

function SetInputOptions(item: FormItem, elseOptions: InputOptions) {
    item.input = {
        ...elseOptions,
    };
}

function SetRegulars(regularTypes: RegularType[] | undefined, item: FormItem) {
    if (regularTypes && regularTypes.length > 0) {
        regularTypes.forEach(temp => {
            switch (Number(temp)) {
                case RegularType.MobilePhone:
                    item.rules?.push({
                        pattern: /^1(3|4|5|6|7|8|9)\d{9}$/,
                        message: '请填写正确的手机号'
                    });
                    break;
            }
        });
    }
}