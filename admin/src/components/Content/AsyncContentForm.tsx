import React, { useEffect, useState } from 'react';
import { AsyncContentFormProps, ContentFormProps, ColumnField, AsyncContentFormAction } from './data';
import { GetColumnContentFields } from './service';
import { message } from 'antd';
import ContentForm from './ContentForm';

const AsyncContentForm: React.FC<AsyncContentFormProps> = ({ columnNum, itemNum, actionRef, isSeo }) => {
    const [formFields, setFormFields] = useState<ColumnField[]>([]);

    const action: AsyncContentFormAction = {
        reload: () => {
            GetColumnContentFields(columnNum).then(res => {
                if (res.isSuccess && res.data) {
                    const { fields } = res.data;
                    setFormFields(fields)
                } else {
                    message.error(res.message || '获取表单数据失败');
                }
            })
        },
        submit: () => {

        }
    }

    if (actionRef && typeof actionRef === 'function') {
        actionRef(action);
    } else if (actionRef && typeof actionRef !== 'function') {
        actionRef.current = action;
    }

    useEffect(() => {
        if (columnNum) {
            action.reload();
        }
    }, [columnNum])

    const formProps: ContentFormProps = {
        columnFields: formFields,
        columnNum,
        itemNum,
        isSeo,
        onFinish: (value) => {
            return new Promise(resolve => {

            })
        }
    }

    return <ContentForm {...formProps} />
}

export default AsyncContentForm;