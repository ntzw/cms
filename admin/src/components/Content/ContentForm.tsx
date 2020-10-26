import React, { useEffect, useState, useRef } from 'react';
import { ContentFormProps, ContentFormAction, ColumnField } from './data';
import DynaminForm, { FormItemType, handleFormData, DefaultSplit } from '@/components/DynamicForm';
import { DynaminFormProps, FormItem, DynaminFormAction } from '@/components/DynamicForm/data';
import { lowerCaseFieldName } from '@/utils/utils';
import { Row, Col, Card, Spin } from 'antd';
import { EditType } from '@/pages/cms/columnlist/components/modelfieldadd';

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
    dataSource: string;
    labelFieldName: string;
    valueFieldName: string;
}

interface CascaderOptions extends BaseOptions {
    dataSource: string;
    labelFieldName: string;
    changeOnSelect: boolean;
}

interface UploadOptions extends BaseOptions {
    uploadType: "image" | "file";
    uploadMax: number;
    action: string;
    accept: string[];
}

interface SwitchOptions extends BaseOptions {
    checkedChildren: string;
    unCheckedChildren: string;
}

interface EditOptions extends BaseOptions {
    editType: EditType;
}

enum RegularType {
    MobilePhone = 1,
}

const ContentForm: React.FC<ContentFormProps> = ({
    columnNum,
    itemNum,
    actionRef,
    columnFields,
    onFinish,
    isSeo = true,
    isCategory = false,
    isAllowTop = false
}) => {
    const [formFields, setFormFields] = useState<FormItem[]>([]);
    const [seoFormFields] = useState<FormItem[]>([{
        label: '标题',
        name: 'seoTitle'
    }, {
        label: '关键词',
        name: 'seoKeyword',
        type: FormItemType.textArea,
        textarea: {
            maxLength: 500,
            rows: 6,
        }
    }, {
        label: '描述',
        name: 'seoDesc',
        type: FormItemType.textArea,
        textarea: {
            maxLength: 500,
            rows: 6,
        }
    }])
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
            let data = handleFormData(formFields, value);
            seoFormAction?.current?.setValue({
                seoDesc: data.seoDesc,
                seoKeyword: data.seoKeyword,
                seoTitle: data.seoTitle,
            });
            formAction.current?.setValue(data);

        },
        reoladFieldItem: () => {
            if (formFields.length > 0)
                formAction.current?.reloadFieldItem();
        },
        loading: (status) => {
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
            let fields = parsingColumnFields(columnFields, columnNum)
            if (isCategory) {
                fields.splice(0, 0, {
                    label: '所属类别',
                    name: 'categoryNum',
                    type: FormItemType.cascader,
                    dataAction: '/Api/CMS/Category/CascaderData',
                    cascader: {
                        options: []
                    }
                })
            }

            if (isAllowTop) {
                fields.push({
                    label: '是否置顶',
                    name: 'isTop',
                    type: FormItemType.switch,
                    switch: {
                        checkedChildren: '已置顶',
                        unCheckedChildren: '未置顶',
                    }
                })
            }

            setFormFields(fields);
        }
    }, [columnFields]);

    const formAction = useRef<DynaminFormAction>();
    const seoFormAction = useRef<DynaminFormAction>();
    const formProps: DynaminFormProps<any> = {
        fields: formFields,
        actionRef: formAction,
        onFinish: (value) => {
            const seoValue = seoFormAction.current?.getValue();
            return new Promise(resolve => {
                setLoading({
                    ...loading,
                    submit: true,
                })

                const data = {
                    ...value,
                    ...seoValue
                };

                if (Array.isArray(data.categoryNum)) {
                    data.categoryNum = data.categoryNum[data.categoryNum.length - 1];
                }
                onFinish(handleSubmitFormData(columnFields, data)).then(res => {
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
        fields: seoFormFields,
    }

    return <Spin tip="表单数据加载中，请稍后" spinning={loading.load}>
        <Spin
            spinning={loading.submit}
            tip="数据提交中，请稍后..."
        >
            <Row>
                <Col flex="1 1 400px">
                    <DynaminForm
                        {...formProps}
                        name="contentform"
                        layout={{ labelCol: { span: 4 }, wrapperCol: { span: 18 } }}
                        fieldActionParams={(field) => {
                            switch (field.name.toLocaleLowerCase()) {
                                case 'categorynum':
                                    return {
                                        columnNum,
                                    }
                                default:
                                    return {}
                            }
                        }}
                    />
                </Col>
                {isSeo && <Col flex="380px">
                    <Card title="SEO信息" bodyStyle={{ padding: 10, paddingTop: 24 }}>
                        <DynaminForm
                            {...seoFormProps}
                            name="setForm"
                            layout={{ labelCol: { span: 5 }, wrapperCol: { span: 18 } }} />
                    </Card>
                </Col>}
            </Row>
        </Spin>
    </Spin>
}

export default ContentForm;

/**
 * 解析栏目字段
 * @param fields 
 * @param columnNum 
 */
export function parsingColumnFields(fields: ColumnField[], columnNum: string) {
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
                        validator: (_, value) => {
                            return new Promise((resolve, reject) => {
                                if (!value) {
                                    reject('请输入正文内容');
                                } else {
                                    resolve();
                                }
                            })
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
                SetSelectOptions(item, elseOptions as SelectOptions, columnNum);
                break;
            case FormItemType.upload:
                SetUploadOptions(item, elseOptions as UploadOptions);
                break;
            case FormItemType.switch:
                SetSwitchOptions(item, elseOptions as SwitchOptions)
                break;
            case FormItemType.cascader:
                SetCascaderOptions(item, elseOptions as CascaderOptions, columnNum);
                break;
            case FormItemType.editor:
                const editOptions = elseOptions as EditOptions;

                item.editType = editOptions.editType;
                break;
        }

        return item;
    })
}

function SetCascaderOptions(item: FormItem, elseOptions: CascaderOptions, columnNum: string) {
    const { dataSource, labelFieldName, changeOnSelect } = elseOptions;
    switch (dataSource) {
        case 'currentColumn':
            item.dataAction = '/Api/CMS/Content/CascaderData';
            item.dataParams = {
                columnNum,
                labelFieldName,
                currentFieldName: item.name,
            }
            break;
    }

    item.cascader = {
        changeOnSelect: changeOnSelect,
        options: [],
    }
}

function SetSwitchOptions(item: FormItem, switchOptions: SwitchOptions) {
    item.switch = {
        checkedChildren: switchOptions.checkedChildren,
        unCheckedChildren: switchOptions.unCheckedChildren,
    }
}

function SetUploadOptions(item: FormItem, elseOptions: UploadOptions) {
    item.upload = {
        type: elseOptions.uploadType,
        max: elseOptions.uploadMax,
        action: elseOptions.action,
        accept: elseOptions.accept?.join(','),
    }
}

function SetInputOptions(item: FormItem, elseOptions: InputOptions) {
    item.input = {
        ...elseOptions,
    };
}

function SetSelectOptions(item: FormItem, elseOptions: SelectOptions, columnNum: string) {
    const { dataSource, options, labelFieldName, valueFieldName } = elseOptions;

    item.select = {};
    switch (dataSource) {
        case 'custom':
            item.select.options = options.map(temp => {
                return {
                    label: temp,
                    value: temp,
                }
            });
            break;
        case 'currentColumn':
            item.dataAction = '/Api/CMS/Content/SelectData';
            item.dataParams = {
                columnNum,
                labelFieldName: labelFieldName,
                valueFieldName: valueFieldName,
            }
            break;

        default:
            break;
    }

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

/**
 * 处理表单提交数据
 * @param columnFields 栏目字段
 * @param value 旧数据
 */
export function handleSubmitFormData(columnFields: ColumnField[], value: any) {
    columnFields.forEach(field => {
        const fieldName = lowerCaseFieldName(field.name);
        const fieldValue = value[fieldName];
        const options = JSON.parse(field.options);

        switch (field.optionType) {
            case FormItemType.tags:
                if (Array.isArray(fieldValue)) {
                    value[fieldName] = fieldValue.join(DefaultSplit)
                }
                break;
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
            case FormItemType.cascader:
                if (Array.isArray(fieldValue)) {
                    value[fieldName] = fieldValue[fieldValue.length - 1];
                }
                break;
        }
    });

    return value;
}