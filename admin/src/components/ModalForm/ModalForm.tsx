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
                onFinish(value).then(() => {
                    setLoading({
                        ...loading,
                        submit: false,
                    })
                    resolve();
                })
            })
        }
    }

    const userAction: DynaminFormAction = {
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
            if (!isUpdate) {
                editAction.current?.clear();
            }

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