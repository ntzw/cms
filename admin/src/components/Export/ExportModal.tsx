import React, { useState } from 'react'
import { Alert, Modal, Result, Spin } from 'antd'
import { connect } from 'umi'
import Button from 'antd/es/button';
import { exportData } from './service';

interface ExportModalProps {
    url: string;
    tips: React.ReactNode;
}

export interface ExportModalInstance {
    export: (data?: { [key: string]: any }) => void;
}

const ExportModal: React.ForwardRefRenderFunction<ExportModalInstance, ExportModalProps> = ({ url, tips }, ref) => {

    const [submitLoading, setSubmitLoading] = useState(false);
    const [modalVisible, setModalVisible] = useState(false);
    const [exportResult, setExportResult] = useState<{
        isSuccess: boolean;
        message: string;
        downloadUrl: string;
    }>({
        isSuccess: false,
        message: '',
        downloadUrl: ''
    })

    React.useImperativeHandle(ref, () => ({
        export: (data) => {
            Modal.confirm({
                title: '系统提示',
                content: tips,
                okText: '确定导出',
                cancelText: '取消',
                onOk: () => {
                    setModalVisible(true);
                    setSubmitLoading(true);

                    exportData(url, data).then(res => {
                        if (res) {
                            setExportResult({
                                isSuccess: res.isSuccess,
                                message: res.message,
                                downloadUrl: res.data || ''
                            })
                            setSubmitLoading(false);
                        }
                    })
                }
            })
        }
    }));

    return <Modal
        title="数据导出"
        visible={modalVisible}
        footer={null}
        closable={false}
    >
        {submitLoading ? <Spin spinning={submitLoading} tip="数据打包中，请耐心等待，请不要关闭该窗口....">
            <Alert
                message="数据导出"
                description="数据打包中，请耐心等待，请不要关闭该窗口"
                type="info"
                showIcon
            />
        </Spin> : (exportResult.isSuccess ? <Result
            status="success"
            title="操作成功"
            subTitle={<span>数据打包成功，<a href={exportResult.downloadUrl} target="_blank">点击下载文件</a></span>}
            extra={[<Button onClick={() => { setModalVisible(false) }}>关闭窗口</Button>]}
        /> : <Result
                status="error"
                title="导出失败"
                subTitle={exportResult.message}
                extra={[<Button onClick={() => { setModalVisible(false) }}>关闭窗口</Button>]}
            />)}
    </Modal>
}


const ExportModalTemp = React.forwardRef<ExportModalInstance, ExportModalProps>(ExportModal);
export default connect(null, null, null, { forwardRef: true })(ExportModalTemp);