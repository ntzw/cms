import React, { useEffect, useState, useRef } from 'react';
import { ContentFormProps, ContentFormAction, ColumnField } from './data';
import DynaminForm, { FormItemType, handleFormData } from '@/components/DynamicForm';
import { DynaminFormProps, FormItem, DynaminFormAction } from '@/components/DynamicForm/data';
import { lowerCaseFieldName } from '@/utils/utils';
import { Row, Col, Card, Spin } from 'antd';

interface BaseOptions {
    required?: boolean;
    regularTypes?: RegularType[];
    extra?: string;
}

interface InputOptions extends BaseOptions {
    maxLength?: number;
    addonBefore?: string;
    addonAfter?: string;
}

interface SelectOptions extends BaseOptions {
    options: string[];
    multiple: boolean;
}

enum RegularType {
    MobilePhone = 1,
}

const ContentForm: React.FC<ContentFormProps> = ({ columnNum, itemNum, actionRef, columnFields, onFinish, isSeo = true }) => {
    const [formFields, setFormFields] = useState<FormItem[]>([]);
    const [loading, setLoading] = useState({
        submit: false,
        load: false,
    })

    const action: ContentFormAction = {
        clear: () => {
            formAction.current?.clear();
            seoFormAction.current?.clear();
        },
        submit: () => {
            formAction.current?.submit();
        },
        setValue: (value) => {
            const newData = handleFormData(parsingColumnFields(columnFields), value);
            formAction.current?.setValue(newData);
            seoFormAction.current?.setValue(newData);
        },
        loading: (status) => {
            console.info('setLoading', status)
            setLoading({
                ...loading,
                load: status
            })
        }
    }

    if (actionRef && typeof actionRef === 'function') {
        actionRef(action);
    } else if (actionRef && typeof actionRef !== 'function') {
        actionRef.current = action;
    }

    useEffect(() => {
        if (columnFields) {
            setFormFields(parsingColumnFields(columnFields))
        }
    }, [columnFields])

    const formAction = useRef<DynaminFormAction>();
    const seoFormAction = useRef<DynaminFormAction>();
    const formProps: DynaminFormProps<any> = {
        fields: formFields,
        actionRef: formAction,
        onFinish: (value) => {
            return new Promise(resolve => {
                setLoading({
                    ...loading,
                    submit: true,
                })
                const seoValue = seoFormAction.current?.getValue();
                onFinish(handleSubmitFormData(columnFields, { ...value, ...seoValue })).then(res => {
                    setLoading({
                        ...loading,
                        submit: false,
                    })
                })
            })
        }
    }

    const seoFormProps: DynaminFormProps<any> = {
        actionRef: seoFormAction,
        fields: [{
            label: '标题',
            name: 'seoTitle'
        }, {
            label: '关键词',
            name: 'seoKeyword',
            type: FormItemType.textArea,
            textarea: {
                maxLength: 500
            }
        }, {
            label: '描述',
            name: 'seoDesc',
            type: FormItemType.textArea,
            textarea: {
                maxLength: 500
            }
        }],
    }

    return <Spin tip="表单数据加载中，请稍后" spinning={loading.load}>
        <Spin
            spinning={loading.submit}
            tip="数据提交中，请稍后..."
        >
            <Row>
                <Col span={isSeo ? 15 : 24}>
                    <DynaminForm {...formProps} layout={{ labelCol: { span: 4 }, wrapperCol: { span: 18 } }} />
                </Col>
                {isSeo ? <Col span={9}>
                    <Card title="SEO信息">
                        <DynaminForm {...seoFormProps} layout={{ labelCol: { span: 5 }, wrapperCol: { span: 18 } }} />
                    </Card>
                </Col> : null}
            </Row>
        </Spin>
    </Spin>
}

export default ContentForm;

export function parsingColumnFields(fields: ColumnField[]) {
    return fields.map((temp): FormItem => {
        const options: BaseOptions = JSON.parse(temp.options);
        const { required, regularTypes, extra, ...elseOptions } = options;

        const item: FormItem = {
            label: temp.explain,
            name: lowerCaseFieldName(temp.name),
            type: temp.optionType,
            extra: extra,
            rules: [],
        };

        if (required) {
            switch (item.type) {
                case FormItemType.editor:
                    item.validateTrigger = 'onBlur';
                    item.rules?.push({
                        required: true,
                        validator: (_, value, callback) => {
                            if (value.isEmpty()) {
                                callback('请输入正文内容');
                            }
                            else {
                                callback(undefined);
                            }
                        }
                    });
                    break;

                default:
                    item.rules?.push({
                        required: true,
                        message: `${temp.explain} 为必填项`
                    });
                    break;
            }
        }

        SetRegulars(regularTypes, item);
        switch (item.type) {
            case FormItemType.input:
                SetInputOptions(item, elseOptions as InputOptions);
                break;
            case FormItemType.select:
                SetSelectOptions(item, elseOptions as SelectOptions);
                break;

            default:
                break;
        }

        return item;
    })
}

function SetInputOptions(item: FormItem, elseOptions: InputOptions) {
    item.input = {
        ...elseOptions,
    };
}

function SetSelectOptions(item: FormItem, elseOptions: SelectOptions) {
    item.select = {
        options: elseOptions.options.map(temp => {
            return {
                label: temp,
                value: temp,
            }
        }),
    };

    if (elseOptions.multiple) {
        item.select.mode = 'multiple';
    }
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

export function handleSubmitFormData(columnFields: ColumnField[], value: any) {
    columnFields.forEach(field => {
        const fieldName = lowerCaseFieldName(field.name);
        const fieldValue = value[fieldName];
        const options = JSON.parse(field.options);

        switch (field.optionType) {
            case FormItemType.editor:
                if (typeof fieldValue !== 'string' && fieldValue.toHTML) {
                    value[fieldName] = fieldValue.toHTML();
                }
                break;
            case FormItemType.select:
                {
                    const selectOptions = options as SelectOptions;
                    if (selectOptions.multiple && fieldValue instanceof Array) {
                        value[fieldName] = fieldValue.join(',');
                    }
                }
                break;
        }
    });

    return value;
}