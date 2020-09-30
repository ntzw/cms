import React, { useEffect, useRef, useState } from 'react';
import { ContentEditProps, ColumnContentItem, ContentModelState } from "../data";
import { Drawer, Button, message } from 'antd';
import ContentForm from '@/components/Content/ContentForm';
import { connect } from 'umi';
import styles from '../style.less'
import { ContentFormAction } from '@/components/Content/data';
import { ContentSubmit, GetEditValue } from '../service';
import { SaveOutlined, EnterOutlined } from '@ant-design/icons';

const ContentEditDrawer: React.FC<ContentEditProps> = ({
    visible,
    currentColumn,
    currentColumnNum,
    itemNum,
    columnFields,
    onClose,
    actionRef,
    afterVisibleChange
}) => {

    const formAction = useRef<ContentFormAction>();
    const [isLoadData, setIsLoadData] = useState(false);
    const [editValue, setEditValue] = useState<ColumnContentItem>();
    const [loading, setLoading] = useState({
        submit: false,
    })
    const [isClose, setIsClose] = useState(false);

    useEffect(() => {
        formAction.current?.clear();
        setIsLoadData(true);
    }, [itemNum])

    const action: ContentFormAction = {
        clear: () => {
            formAction.current?.clear();
        },
        submit: () => {
            formAction.current?.submit();
        },
        setValue: (value) => {
            formAction.current?.setValue(value);
        },
        reoladFieldItem: () => {
            formAction.current?.reoladFieldItem();
        },
        loading: (status) => {
            formAction.current?.loading(status);
        }
    }

    if (actionRef && typeof actionRef === 'function') {
        actionRef(action);
    } else if (actionRef && typeof actionRef !== 'function') {
        actionRef.current = action;
    }

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
        afterVisibleChange={(visible) => {
            if (isLoadData && itemNum && currentColumnNum) {
                formAction.current?.loading(true);
                GetEditValue(itemNum, currentColumnNum).then(res => {
                    if (res.isSuccess) {
                        setEditValue(res.data);
                        formAction.current?.setValue(res.data);
                    }

                    setTimeout(() => {
                        formAction.current?.loading(false);
                        setIsLoadData(false);
                    }, 800);
                });
            }

            if (afterVisibleChange)
                afterVisibleChange(visible);
        }}
        footer={<div style={{ textAlign: 'center' }}>
            <Button.Group>
                <Button
                    loading={loading.submit}
                    icon={<EnterOutlined />}
                    type="primary"
                    onClick={() => {
                        setIsClose(true);
                        formAction.current?.submit();
                    }}
                >提交并关闭</Button>
                <Button
                    loading={loading.submit}
                    type="primary"
                    icon={<SaveOutlined />}
                    onClick={() => {
                        setIsClose(false);
                        formAction.current?.submit();
                    }}
                >提交</Button>
            </Button.Group>
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
            columnNum={currentColumnNum || ''}
            isCategory={currentColumn?.isCategory}
            isSeo={currentColumn?.isSeo}
            isAllowTop={currentColumn?.isAllowTop}
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
                        columnNum: currentColumnNum,
                    }

                    ContentSubmit(newValue).then(res => {
                        if (res.isSuccess) {
                            message.success('数据提交成功');
                            if (isClose) {
                                onClose(res.isSuccess);
                            } else if (!itemNum) {
                                formAction.current?.clear();
                            }
                        } else {
                            message.error(res.message || '数据提交失败');
                        }

                        resolve();
                        formAction.current?.reoladFieldItem();
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

export default connect(({ content: { currentColumn, currentColumnNum, columnTableFields } }: { content: ContentModelState }) => {
    return {
        currentColumn,
        currentColumnNum,
        columnFields: columnTableFields && currentColumnNum ? columnTableFields[currentColumnNum] : [],
    }
})(ContentEditDrawer);