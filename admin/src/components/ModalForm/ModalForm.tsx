import React, { useState, useEffect, createRef } from 'react';
import { Modal, Form, Input, Spin, Select } from 'antd';
import { Rule, FormInstance } from 'antd/lib/form';
import { InputProps } from 'antd/lib/input';
import { getFields, getAsyncData } from './service';
import { Store } from 'antd/lib/form/interface';
import { SelectProps } from 'antd/lib/select';

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
    type?: FormItemType;
    rules?: Rule[];
    input?: InputProps;
    password?: InputProps;
    select?: SelectProps<any>;
}

interface AsyncResult<T extends Store> {
    editData: T,
    field: FormItem[];
}

export interface ModalFormAction {
    reload: () => void;
    setSubmitLoading: (loading: boolean) => void;
    close: () => void;
}

enum FormItemType {
    input,
    password,
    select,
}

const layout = {
    labelCol: { span: 6 },
    wrapperCol: { span: 16 },
};

const getFormItem = (item: FormItem) => {
    const type = item.type || FormItemType.input;
    switch (type) {
        case FormItemType.select:
            return <Select {...item.select} />
        case FormItemType.password:
            return <Input.Password {...item.password} />
        default:
            return <Input {...item.input} />
    }
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
    let needAsyncField = false;

    const loadFieldAsyncData = (fields: FormItem[]) => {
        setFormItemData(fields);

        let asyncCount = 0;
        function asyncOver() {
            if (asyncCount === 0) {
                setFormItemData(fields);
                setLoading({
                    ...loading,
                    loadFields: false,
                })
            }
        }

        fields.forEach(item => {
            switch (item.type) {
                case FormItemType.select:
                    asyncCount += 1;
                    getAsyncData(item.dataAction).then(res => {
                        if (item.select)
                            item.select.options = res?.data || [];

                        asyncCount -= 1;
                        asyncOver();
                    })
                    break;

                default:
                    break;
            }
        })

        asyncOver();
    }

    const loadFields = (url: string, params?: { [key: string]: any }) => {
        setLoading({
            ...loading,
            loadFields: true,
        })
        getFields<AsyncResult<T>>(url, params).then(res => {
            if (res?.isSuccess && res.data) {
                setEditData(res.data.editData);
                form.current?.setFieldsValue(res.data.editData);

                loadFieldAsyncData(res.data.field);
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
            if (typeof onClose === 'function')
                onClose();
        }
    }

    return <Modal
        visible={visible}
        title={title}
        width={width}
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
                        rules={item.rules}
                    >
                        {getFormItem(item)}
                    </Form.Item>
                })}
            </Form>
        </Spin>
    </Modal>
}

export default ModalForm;