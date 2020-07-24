import React, { useState, useRef } from 'react';
import { Modal } from 'antd';
import { Store } from 'antd/lib/form/interface';
import { DynaminFormProps, DynaminFormAction } from '../DynamicForm/data';
import DynaminForm from '../DynamicForm';

interface ModalFormProps<T extends Store> extends DynaminFormProps<T> {
    visible: boolean;
    title: string;
    isUpdate: boolean;
    width?: string | number;
    onClose?: () => void;
}

export interface ModalFormAction extends DynaminFormAction {

}

const ModalForm = <T extends Store>(props: ModalFormProps<T>) => {
    const {
        visible,
        title,
        width,
        onClose,
        actionRef,
        isUpdate,
        onFinish,
        ...rest
    } = props;

    const [loading, setLoading] = useState({
        submit: false
    })

    const editAction = useRef<DynaminFormAction>();
    const formProps: DynaminFormProps<T> = {
        ...rest,
        actionRef: editAction,
        onFinish: (value: T) => {
            return new Promise<any>(resolve => {
                setLoading({
                    ...loading,
                    submit: true,
                })
                if (onFinish) {
                    onFinish(value).then(() => {
                        setLoading({
                            ...loading,
                            submit: false,
                        })
                        resolve();
                    })
                }
            })
        }
    }

    const userAction: ModalFormAction = {
        reload: () => {
            editAction.current?.reload();
        },
        reloadFieldItem: () => {
            editAction.current?.reloadFieldItem();
        },
        clear: () => {
            editAction.current?.clear();
        },
        submit: () => {
            editAction.current?.submit();
        },
        setValue: (value) => {
            editAction.current?.setValue(value);
        },
        getValue: () => {
            return editAction.current?.getValue();
        },
        setLoading: (status) => {
            editAction.current?.setLoading(status);
        },
        setOldValue: () => {
            editAction.current?.setOldValue();
        }
    }

    if (actionRef && typeof actionRef === 'function') {
        actionRef(userAction);
    } else if (actionRef && typeof actionRef !== 'function') {
        actionRef.current = userAction;
    }

    return <Modal
        visible={visible}
        title={title}
        width={width}
        maskClosable={false}
        confirmLoading={loading.submit}
        onCancel={() => {
            if (typeof onClose === 'function') {
                onClose();
            }
        }}
        onOk={() => {
            editAction.current?.submit();
        }}
    >
        <DynaminForm
            {...formProps}
        />
    </Modal>
}

export default ModalForm;