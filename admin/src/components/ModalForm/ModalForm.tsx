import React, { useState, useEffect, createRef } from 'react';
import { Modal, Form, Input, Spin } from 'antd';
import { Rule, FormInstance } from 'antd/lib/form';
import { InputProps } from 'antd/lib/input';
import { getFields } from './service';
import { Store } from 'antd/lib/form/interface';

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
    actionRef?: React.MutableRefObject<ActionType | undefined> | ((actionRef: ActionType) => void);
}

interface FormItem {
    label: string;
    name: string;
    type?: FormItemType;
    rules?: Rule[];
    input?: InputProps;
    password?: InputProps;
}

interface AsyncResult<T extends Store> {
    editData: T,
    field: FormItem[];
}

export interface ActionType {
    reload: () => void;
    setSubmitLoading: (loading: boolean) => void;
    close: () => void;
}

enum FormItemType {
    input,
    password,
}

const layout = {
    labelCol: { span: 6 },
    wrapperCol: { span: 16 },
};

const getFormItem = (item: FormItem) => {
    const type = item.type || FormItemType.input;
    switch (type) {
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

    const loadFields = (url: string, params?: { [key: string]: any }) => {
        setLoading({
            ...loading,
            loadFields: true,
        })
        getFields<AsyncResult<T>>(url, params).then(res => {
            if (res?.isSuccess && res.data) {
                setFormItemData(res.data.field);

                setEditData(res.data.editData);
                form.current?.setFieldsValue(res.data.editData)
            }

            setLoading({
                ...loading,
                loadFields: false,
            })
        })
    }

    useEffect(() => {
        if (fields instanceof Array) {
            setFormItemData(fields);
        } else if (typeof fields === 'string' && fields) {
            loadFields(fields, params);
        }

        if (actionRef && typeof actionRef === 'function') {
            actionRef(userAction);
        } else if (actionRef && typeof actionRef !== 'function') {
            actionRef.current = userAction;
        }
    }, [fields, JSON.stringify(params || {})]);

    const userAction: ActionType = {
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