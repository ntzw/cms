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
    const [isLoadData, setIsLoadData] = useState(false);
    const [editValue, setEditValue] = useState<ColumnContentItem>();
    const [loading, setLoading] = useState({
        submit: false,
    })

    useEffect(() => {
        formAction.current?.clear();
        setIsLoadData(true);
    }, [itemNum])

    return <Drawer
        className={styles.contentEditDrawer}
        title={`${(itemNum ? '编辑' : '添加')}内容`}
        visible={visible}
        width="90%"
        placement="left"
        onClose={() => {
            onClose();
        }}
        maskClosable={false}
        afterVisibleChange={() => {
            if (isLoadData && itemNum) {
                formAction.current?.loading(true);
                GetEditValue(itemNum, columnNum).then(res => {
                    if (res.isSuccess) {
                        setEditValue(res.data);
                        formAction.current?.setValue(res.data);
                    }

                    formAction.current?.loading(false);
                    setIsLoadData(false);
                });
            }
        }}
        footer={<div style={{ textAlign: 'center' }}>
            <Button
                loading={loading.submit}
                type="primary"
                onClick={() => {
                    formAction.current?.submit();
                }}
            >提交</Button>
            <Button
                loading={loading.submit}
                danger
                style={{ marginLeft: 10 }}
                onClick={() => {
                    formAction.current?.clear();
                }}
            >重置</Button>
            <Button
                loading={loading.submit}
                style={{ marginLeft: 10 }}
                onClick={() => {
                    onClose();
                }}
            >取消</Button>
        </div>}
    >
        <ContentForm
            columnNum={columnNum}
            columnFields={columnFields}
            actionRef={formAction}
            onFinish={(value) => {
                return new Promise(resolve => {
                    setLoading({
                        ...loading,
                        submit: true,
                    })

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

                        setLoading({
                            ...loading,
                            submit: false,
                        })
                    })
                })
            }}
        />
    </Drawer>
}

export default connect()(ContentEditDrawer);