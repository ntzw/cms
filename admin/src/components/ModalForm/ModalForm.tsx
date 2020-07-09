import React, { useState, useEffect, createRef } from 'react';
import { Modal, Form, Input, Spin, Select, Cascader, Switch } from 'antd';
import { FormInstance, RuleObject } from 'antd/lib/form';
import { InputProps, TextAreaProps, PasswordProps } from 'antd/lib/input';
import { getFields, getAsyncData } from './service';
import { Store } from 'antd/lib/form/interface';
import { SelectProps } from 'antd/lib/select';
import { CascaderProps, CascaderOptionType } from 'antd/lib/cascader';
import { SwitchProps } from 'antd/lib/switch';

interface ModalFormProps<T extends Store> {
    visible: boolean;
    title: string;
    fields: FormItem[] | string;
    onFinish: (value: T) => void;
    params?: { [key: string]: any }
    width?: string | number;
    onClose?: () => void;
    /**
   * 初始化的参数，可以操作 Form
   */
    actionRef?: React.MutableRefObject<ModalFormAction | undefined> | ((actionRef: ModalFormAction) => void);
}

interface FormItem {
    label: string;
    name: string;
    dataAction: string;
    valuePropName?: string;
    split?: string;
    type?: FormItemType;
    rules?: RuleObject[];
    input?: InputProps;
    password?: PasswordProps;
    textarea?: TextAreaProps;
    select?: SelectProps<any>;
    cascader?: CascaderProps;
    switch?: SwitchProps;
}

interface AsyncResult<T extends Store> {
    editData: T,
    field: FormItem[];
}

export interface ModalFormAction {
    reload: () => void;
    clear: () => void;
    setSubmitLoading: (loading: boolean) => void;
    close: () => void;
}

enum FormItemType {
    input,
    password,
    select,
    textArea,
    cascader,
    switch
}

const layout = {
    labelCol: { span: 6 },
    wrapperCol: { span: 16 },
};

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

const ModalForm = <T extends Store>(props: ModalFormProps<T>) => {
    const {
        visible,
        title,
        fields,
        width,
        onClose,
        params,
        actionRef,
        onFinish
    } = props;

    const [editData, setEditData] = useState<T>();
    const [formItemData, setFormItemData] = useState<Array<FormItem>>([]);

    const [loading, setLoading] = useState({
        loadFields: false,
        confirm: false,
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
                    getAsyncData(item.dataAction).then(res => {

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
                    fields.forEach(field => {
                        const fieldName: string = field.name;
                        const oldValue: string | number = data[fieldName];
                        switch (field.type) {
                            case FormItemType.cascader:
                                data[fieldName] = handleCascaderValue(field.cascader?.options, oldValue);
                                break;
                            case FormItemType.select:
                                if (field.select) {
                                    if (field.select.mode === 'tags') {
                                        data[fieldName] = [];
                                        if (field.split && typeof oldValue === 'string')
                                            data[fieldName] = oldValue.split(field.split).filter(temp => !!temp);

                                    }
                                }
                                break;
                            default:
                                break;
                        }
                    })
                    form.current?.setFieldsValue(data);
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
        if (fields instanceof Array) {
            setFormItemData(fields);
        } else if (typeof fields === 'string' && fields) {
            loadFields(fields, params);
        }
    }

    useEffect(() => {
        loadAsyncField();

        if (actionRef && typeof actionRef === 'function') {
            actionRef(userAction);
        } else if (actionRef && typeof actionRef !== 'function') {
            actionRef.current = userAction;
        }
    }, [fields, JSON.stringify(params || {})]);

    const userAction: ModalFormAction = {
        reload: () => {
            if (typeof fields === 'string' && fields) {
                loadFields(fields, params);
            }
        },
        setSubmitLoading: (confirm) => {
            setLoading({
                ...loading,
                confirm,
            })
        },
        close: () => {
            if (typeof onClose === 'function') {
                userAction.clear();
                onClose();
            }
        },
        clear: () => {
            form.current?.resetFields();
        }
    }

    return <Modal
        visible={visible}
        title={title}
        width={width}
        maskClosable={false}
        onCancel={userAction.close}
        confirmLoading={loading.confirm}
        onOk={() => {
            form.current?.submit();
        }}
    >
        <Spin
            spinning={loading.loadFields}
            tip="表单生成中，请稍等...."
        >
            <Form
                {...layout}
                name="basic"
                ref={form}
                onFinish={(value) => {
                    onFinish({
                        ...editData,
                        ...value
                    } as T)
                }}
            >
                {formItemData.map(item => {
                    return <Form.Item
                        key={item.name}
                        label={item.label}
                        name={item.name}
                        valuePropName={item.valuePropName}
                        rules={item.rules?.map((temp) => {
                            if (typeof temp.pattern === 'string') {
                                temp.pattern = new RegExp(temp.pattern);
                            }
                            return temp;
                        })}
                    >
                        {getFormItem(item)}
                    </Form.Item>
                })}
            </Form>
        </Spin>
    </Modal>
}

export default ModalForm;