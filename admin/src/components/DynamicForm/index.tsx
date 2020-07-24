import { DynaminFormProps, FormItem, AsyncResult, DynaminFormAction } from "./data";
import React, { useState, createRef, useEffect } from "react";
import { Spin, Input, Select, Cascader, Switch, Form, Empty } from "antd";
import { CascaderOptionType } from "antd/lib/cascader";
import { Store } from "antd/lib/form/interface";
import { FormInstance } from "antd/lib/form";
import { getFields, getAsyncData } from "./service";
import 'braft-editor/dist/index.css'
import BraftEditor from 'braft-editor'
import styles from './style.less'


export enum FormItemType {
    input,
    password,
    select,
    textArea,
    cascader,
    switch,
    editor,
}

export function GetFormItemTypeName(type: number) {
    switch (type) {
        case FormItemType.password:
            return '密码框';
        case FormItemType.select:
            return '选择器';
        case FormItemType.switch:
            return '开关';
        case FormItemType.textArea:
            return '多行文本框'
        case FormItemType.cascader:
            return '级联选择';
        case FormItemType.editor:
            return '富文本编辑器';
        default:
            return '文本框';
    }
}

const getFormItem = (item: FormItem) => {
    const type = item.type || FormItemType.input;
    switch (type) {
        case FormItemType.switch:
            return <Switch {...item.switch} />
        case FormItemType.cascader:
            return <Cascader {...item.cascader} />
        case FormItemType.textArea:
            return <Input.TextArea {...item.textarea} />
        case FormItemType.select:
            return <Select {...item.select} />
        case FormItemType.password:
            return <Input.Password {...item.password} />
        case FormItemType.editor:
            return <BraftEditor className={styles.myEditor} placeholder="请输入正文内容" />;
        default:
            return <Input {...item.input} />
    }
}

const handleCascaderValue = (options: CascaderOptionType[] | undefined, value: any): any[] => {
    var newData = [];
    if (options && value) {
        const last = (function setNewData(children): CascaderOptionType | null {
            for (let index = 0; index < children.length; index += 1) {
                const element = children[index];
                if (element.value === value) {
                    return element;
                }
                if (element.children && element.children.length > 0) {
                    const temp = setNewData(element.children);
                    if (temp) {
                        newData.unshift(temp.value);
                        return element;
                    }
                }
            }
            return null;
        })(options);
        if (last) {
            newData.unshift(last.value);
        }
    }
    return newData;
}

const DynaminForm = <T extends Store>(props: DynaminFormProps<T>) => {
    const {
        onFinish,
        fields,
        params,
        fieldActionParams,
        actionRef,
        layout = {
            labelCol: { span: 6 },
            wrapperCol: { span: 16 },
        }
    } = props;

    const [editData, setEditData] = useState<T>();
    const [formItemData, setFormItemData] = useState<Array<FormItem>>([]);

    const [loading, setLoading] = useState({
        loadFields: false,
    });

    const [form] = useState(createRef<FormInstance>());
    const loadFieldAsyncData = (fields: FormItem[]): Promise<FormItem[]> => {
        return new Promise(resolve => {
            setFormItemData(fields);

            let asyncCount = 0;
            function asyncOver() {
                if (asyncCount === 0) {
                    setFormItemData(fields);
                    resolve(fields);
                }
            }

            function asyncData(item: FormItem) {
                if (item.dataAction) {
                    asyncCount += 1;
                    getAsyncData(item.dataAction, fieldActionParams && fieldActionParams(item)).then(res => {

                        switch (item.type) {
                            case FormItemType.select:
                                if (item.select)
                                    item.select.options = res?.data || [];
                                break;
                            case FormItemType.cascader:
                                if (item.cascader)
                                    item.cascader.options = res?.data || [];
                                break;
                        }

                        asyncCount -= 1;
                        asyncOver();
                    })
                }
            }

            fields.forEach(item => {
                switch (item.type) {
                    case FormItemType.select:
                    case FormItemType.cascader:
                        asyncData(item);
                        break;
                    case FormItemType.editor:
                        if (item.rules) {
                            item.rules = item.rules.map(temp => {
                                if (temp.required) {
                                    item.validateTrigger = 'onBlur'
                                    temp.validator = (_, value, callback) => {
                                        if (value.isEmpty()) {
                                            callback(`请输入 ${item.label}`)
                                        } else {
                                            callback()
                                        }
                                    }
                                }

                                return temp;
                            })
                        }
                        break;
                }
            })

            asyncOver();
        });
    }

    const loadFields = (url: string, params?: { [key: string]: any }) => {
        setLoading({
            ...loading,
            loadFields: true,
        })
        getFields<AsyncResult<T>>(url, params).then(res => {
            if (res?.isSuccess && res.data) {
                setEditData(res.data.editData);

                let data: any = { ...res.data.editData };
                loadFieldAsyncData(res.data.field).then(fields => {
                    form.current?.setFieldsValue(handleFormData(fields, data));
                    setLoading({
                        ...loading,
                        loadFields: false,
                    })
                });
            } else {
                setLoading({
                    ...loading,
                    loadFields: false,
                })
            }
        })
    }

    const loadAsyncField = () => {
        userAction.clear();
        if (fields instanceof Array) {
            setFormItemData(fields);
        } else if (typeof fields === 'string' && fields) {
            loadFields(fields, params);
        }
    }

    useEffect(() => {
        loadAsyncField();
    }, [fields, JSON.stringify(params || {})]);

    const userAction: DynaminFormAction = {
        reload: () => {
            if (typeof fields === 'string' && fields) {
                loadFields(fields, params);
            }
        },
        reloadFieldItem: () => {
            loadFieldAsyncData(formItemData);
        },
        clear: () => {
            form.current?.resetFields();
        },
        submit: () => {
            form.current?.submit();
        },
        setValue: (value) => {
            form.current?.setFieldsValue(value);
        },
        getValue: () => {
            return form.current?.getFieldsValue();
        },
        setLoading: (status) => {
            setLoading({
                ...loading,
                loadFields: status
            })
        },
        setOldValue: () => {
            userAction.setValue(handleFormData(formItemData, editData));
        }
    }

    if (actionRef && typeof actionRef === 'function') {
        actionRef(userAction);
    } else if (actionRef && typeof actionRef !== 'function') {
        actionRef.current = userAction;
    }

    return <Spin
        spinning={loading.loadFields}
        tip="表单生成中，请稍等...."
    >
        {formItemData.length > 0 ? <Form
            {...layout}
            name="basic"
            ref={form}
            onFinish={(value) => {
                if (onFinish) {
                    onFinish({
                        ...editData,
                        ...value
                    } as T)
                }
            }}
        >
            {formItemData.map(item => {
                return <Form.Item
                    key={item.name}
                    label={item.label}
                    name={item.name}
                    extra={item.extra ? <span style={{ color: 'green' }}>{item.extra}</span> : null}
                    valuePropName={item.valuePropName}
                    validateTrigger={item.validateTrigger}
                    rules={item.rules?.map((temp) => {
                        if (temp?.pattern && typeof temp.pattern === 'string') {
                            temp.pattern = new RegExp(temp.pattern);
                        }
                        return temp;
                    })}
                >
                    {getFormItem(item)}
                </Form.Item>
            })}
        </Form> : <Empty description="未设置表单字段" />}
    </Spin>
}

export default DynaminForm;

export function handleFormData(fields: FormItem[], oldData: any): any {
    const newData = { ...oldData };
    fields.forEach(field => {
        const fieldName: string = field.name;
        const oldValue: string | number = newData[fieldName];
        switch (field.type) {
            case FormItemType.cascader:
                newData[fieldName] = handleCascaderValue(field.cascader?.options, oldValue);
                break;
            case FormItemType.select:
                if (field.select) {
                    switch (field.select.mode) {
                        case 'tags':
                        case 'multiple':
                            newData[fieldName] = [];
                            if (typeof oldValue === 'string')
                                newData[fieldName] = oldValue.split(field.split || ',').filter(temp => !!temp);
                            break;
                    }
                }
                break;
            case FormItemType.editor:
                newData[fieldName] = oldValue && BraftEditor.createEditorState(oldValue);
                break;
            default:
                break;
        }
    });
    return newData;
}
