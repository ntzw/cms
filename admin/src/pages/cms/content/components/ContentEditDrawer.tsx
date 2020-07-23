import React, { useEffect, useRef, useState } from 'react';
import { ContentEditProps, ColumnContentItem } from "../data";
import { Drawer, Button, message } from 'antd';
import ContentForm from '@/components/Content/ContentForm';
import { connect } from 'umi';
import styles from '../style.less'
import { ContentFormAction } from '@/components/Content/data';
import { submit, GetEditValue } from '../service';

const ContentEditDrawer: React.FC<ContentEditProps> = ({
    visible,
    columnNum,
    itemNum,
    columnFields,
    onClose
}) => {

    const formAction = useRef<ContentFormAction>();
    const [editValue, setEditValue] = useState<ColumnContentItem>();

    useEffect(() => {
        formAction.current?.clear();
        if (itemNum) {
            GetEditValue(itemNum, columnNum).then(res => {
                if (res.isSuccess) {
                    setEditValue(res.data);
                    formAction.current?.setValue(res.data);
                }
            });
        }
    }, [itemNum])



    return <Drawer
        className={styles.contentEditDrawer}
        title={`${(itemNum ? '编辑' : '添加')}内容`}
        visible={visible}
        width="80%"
        onClose={() => {
            onClose();
        }}
        maskClosable={false}
    >
        <ContentForm
            columnNum={columnNum}
            columnFields={columnFields}
            actionRef={formAction}
            onFinish={(value) => {
                return new Promise(resolve => {
                    const newValue = {
                        ...editValue,
                        ...value,
                        num: itemNum,
                        columnNum,
                    }
                    submit(newValue).then(res => {
                        if (res.isSuccess) {
                            message.success('数据提交成功');
                            onClose(res.isSuccess);
                        } else {
                            message.error(res.message || '数据提交失败');
                        }

                        resolve();
                    })
                })
            }}
        />
        <div style={{ width: 215, margin: '0 auto' }}>
            <Button
                type="primary"
                onClick={() => {
                    formAction.current?.submit();
                }}
            >提交</Button>
            <Button
                danger
                style={{ marginLeft: 10 }}
                onClick={() => {
                    formAction.current?.clear();
                }}
            >重置</Button>
            <Button
                style={{ marginLeft: 10 }}
                onClick={() => {
                    onClose();
                }}
            >取消</Button>
        </div>
    </Drawer>
}

export default connect()(ContentEditDrawer);