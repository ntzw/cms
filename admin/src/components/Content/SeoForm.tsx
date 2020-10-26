import { message, Modal, Spin } from 'antd';
import React, { FC, useEffect, useRef, useState } from 'react'
import DynaminForm, { FormItemType } from '../DynamicForm';
import { DynaminFormAction, DynaminFormProps, FormItem } from '../DynamicForm/data';
import { SeoAction } from './service';

interface SeoFormProps {
    visible: boolean;
    getDataUrl: string;
    setDataUrl: string;
    param?: { [key: string]: any };
    onClose: () => void;
}

export interface SeoInfo {
    seoTitle: string;
    seoKeyword: string;
    seoDesc: string;
}

const SeoForm: FC<SeoFormProps> = ({ visible, onClose, getDataUrl, setDataUrl, param }) => {
    const [loading, setLoading] = useState(false);
    const [submitLoading, setSubmitLoading] = useState(false);
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
    const seoFormAction = useRef<DynaminFormAction>();
    const seoFormProps: DynaminFormProps<any> = {
        actionRef: seoFormAction,
        fields: seoFormFields,
        onFinish: (value: any) => {
            return new Promise(resolve => {
                const data = {
                    ...param,
                    ...value,
                };

                setSubmitLoading(true);
                SeoAction(setDataUrl, data).then(res => {
                    if (res.isSuccess) {
                        message.success('修改成功');
                        onClose();
                    } else {
                        message.error(res.message);
                    }

                    setSubmitLoading(false);
                })
            });
        }
    }

    const loadData = () => {
        if (getDataUrl) {
            setLoading(true);
            SeoAction(getDataUrl, param).then(res => {
                if (res.isSuccess) {
                    seoFormAction.current?.setValue(res.data);
                }

                setLoading(false);
            })
        }
    }

    useEffect(() => {
        loadData();
    }, [getDataUrl, JSON.stringify(param)])

    return <Modal
        title="SEO设置"
        visible={visible}
        confirmLoading={submitLoading}
        onCancel={() => {
            onClose();
        }}
        onOk={() => {
            seoFormAction.current?.submit();
        }}
    >
        <Spin spinning={loading} tip="加载数据中..." >
            <DynaminForm
                {...seoFormProps}
                name="setForm"
                layout={{ labelCol: { span: 5 }, wrapperCol: { span: 18 } }} />
        </Spin>
    </Modal>
}

export default SeoForm;