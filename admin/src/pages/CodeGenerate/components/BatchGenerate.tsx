import React, { useState, useEffect } from 'react';
import { TableType, connect, ModelState } from 'umi';
import { Modal, Table, Steps, Button, Card, Result } from 'antd';
import { TemplateItem } from '..';
import { PauseCircleOutlined, LoadingOutlined, CheckCircleOutlined, CloseCircleOutlined } from '@ant-design/icons';

import jsZip from 'jszip'
import { saveAs } from 'file-saver'
import { buildTemplateValue, getModalName } from './Generate';


interface BatchGenerateProps {
    tableData: TableType[];
    visible: boolean;
    templateData?: TemplateItem[];
    onClose: () => void;
}

interface TimelineDataItem {
    title: string;
    status: 'wait' | 'loading' | 'success' | 'error';
}

const BatchGenerate: React.FC<BatchGenerateProps> = ({ visible, tableData, onClose, templateData }) => {
    const [checkedRows, setCheckedRows] = useState<TableType[]>([]);
    const [currentStep, setCurrentStep] = useState(0);
    const [timelineData, setTimelineData] = useState<TimelineDataItem[]>([]);
    const [currentTimeline, setCurrentTimeline] = useState(0);

    const renderStepContent = () => {

        switch (currentStep) {
            case 0:
                return <Table
                    bordered={true}
                    pagination={false}
                    size="small"
                    rowKey="name"
                    rowSelection={{
                        type: 'checkbox',
                        onChange: (selectedRowKeys, selectedRows) => {
                            setCheckedRows(selectedRows);
                        },
                    }}
                    columns={[{
                        dataIndex: 'name',
                        title: '表名',
                    }]}
                    dataSource={tableData}
                />;
            case 1:
                return <Steps
                    direction="vertical"
                    current={currentTimeline}
                >
                    {timelineData.map(temp => {
                        return <Steps.Step key={temp.title} title={temp.title} icon={getTimelineDot(temp.status)} />;
                    })}
                </Steps>
            case 2:
                return <Result
                    status="success"
                    title="代码生成成功，等待下载"
                />

            default:
                return <></>
        }
    }

    const getTimelineDot = (status: TimelineDataItem['status']) => {
        switch (status) {
            case 'wait':
                return <PauseCircleOutlined />
            case 'loading':
                return <LoadingOutlined />
            case 'success':
                return <CheckCircleOutlined />
            case 'error':
                return <CloseCircleOutlined />
        }
    }

    const renderModalFooter = () => {
        switch (currentStep) {
            case 0:
                return <Button type="primary" disabled={!checkedRows || checkedRows.length <= 0} onClick={() => {
                    setCurrentStep(1);
                }} >生成</Button>

            default:
                return <></>
        }
    }

    const templateDataToTimelineData = () => {
        setTimelineData(templateData?.map(temp => {
            return {
                title: temp.title,
                status: 'wait'
            }
        }) || []);
    }

    const buildCode = () => {
        var zip = new jsZip();
        buildCodeValue(0, zip);
    }

    const buildCodeValue = (currentTempalteIndex: number, zip: jsZip) => {
        if (!templateData) return;

        const tempalteInfo = templateData[currentTempalteIndex];
        if (!tempalteInfo) {
            setCurrentStep(2);
            zip.generateAsync({ type: "blob" }).then(function (content) {
                // content就是blob数据，这里以example.zip名称下载    
                // 使用了FileSaver.js  

                saveAs(content, "example.zip");
            });
            return;
        };

        const timeline = [...timelineData];
        const timelineInfo = timeline[currentTempalteIndex];
        timelineInfo.status = 'loading';
        setTimelineData(timeline);
        setCurrentTimeline(currentTempalteIndex);

        const names = tempalteInfo.title.split('.');
        const ext = names[names.length - 1];
        const templateName = names[0];

        const folder = zip.folder(templateName);
        checkedRows.forEach(item => {
            const value = buildTemplateValue(tempalteInfo.template, item);
            folder?.file(`${getModalName(item.name)}.${ext}`, value)
        })

        setTimeout(() => {
            timeline[currentTempalteIndex].status = 'success';
            setTimelineData([...timeline]);
            buildCodeValue(currentTempalteIndex + 1, zip);
        }, 1000);
    }

    useEffect(() => {
        switch (currentStep) {
            case 1:
                buildCode();
                break;
        }
    }, [currentStep])

    useEffect(() => {
        templateDataToTimelineData();
    }, [templateData])

    useEffect(() => {
        templateDataToTimelineData();
    }, [])

    return <Modal
        title="批量生成代码"
        visible={visible}
        footer={renderModalFooter()}
        onCancel={() => {
            setTimelineData([]);
            setCurrentStep(0);
            setCurrentTimeline(0)
            setCheckedRows([]);
            onClose();
        }}
    >
        <Steps size="small" current={currentStep}>
            <Steps.Step title="选择数据表" />
            <Steps.Step title="数据生成中" />
            <Steps.Step title="生成成功" />
        </Steps>
        <Card bordered={false}>
            {renderStepContent()}
        </Card>
    </Modal>
}

export default connect(({ codeGenerate: { tableData } }: { codeGenerate: ModelState }) => {
    return {
        tableData,
    }
})(BatchGenerate);
