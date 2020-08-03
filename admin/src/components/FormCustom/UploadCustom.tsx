import React, { useState, useEffect } from 'react';
import { UploadCustomProps } from './data';
import { Upload, Modal, Button } from 'antd';
import { UploadFile, UploadChangeParam } from 'antd/lib/upload/interface';
import { PlusOutlined, UploadOutlined } from '@ant-design/icons';
import { HandleResult } from '@/utils/request';
import defaultConfig from '../../../config/defaultSettings';

function getBase64(file: any) {
    return new Promise<string>((resolve, reject) => {
        const reader = new FileReader();
        reader.readAsDataURL(file);
        reader.onload = () => resolve(reader.result as string || '');
        reader.onerror = error => reject(error);
    });
}

function getUrlFileName(url: string) {
    if (!url) return '';

    return url.substring(url.lastIndexOf('/') + 1);
}

interface UploadImageProps {
    action: string;
    max?: number;
    value?: string;
    accept?: string;
    onChange?: (value: string) => void;
}

const UploadFileDOM: React.FC<UploadImageProps> = ({
    action,
    onChange,
    max = 10,
    value,
    accept
}) => {
    const [fileList, setFileList] = useState<UploadFile<any>[]>([]);
    const uploadButton = (
        <Button>
            <UploadOutlined /> 上传文件
        </Button>
    );

    useEffect(() => {
        if (value) {
            SetUploadValue(value, setFileList, fileList);
        }
    }, [value])

    useEffect(() => {
        SetValue(fileList, onChange);
    }, [fileList])

    return <div>
        <Upload
            action={action}
            accept={accept}
            fileList={fileList}
            onChange={UploadOnChange(max, setFileList)}
        >
            {fileList.length >= max ? null : uploadButton}
        </Upload>
    </div>
}

const UploadImageDOM: React.FC<UploadImageProps> = ({
    action,
    onChange,
    max = 10,
    value,
    accept,
}) => {
    const [fileList, setFileList] = useState<UploadFile<any>[]>([]);
    const [previewVisible, setPreviewVisible] = useState(false);
    const [previewTitle, setPreviewTitle] = useState('预览照片');
    const [previewImage, setPreviewImage] = useState('');
    const uploadButton = (
        <div>
            <PlusOutlined />
            <div className="ant-upload-text">上传</div>
        </div>
    );

    useEffect(() => {
        if (value) {
            SetUploadValue(value, setFileList, fileList);
        }
    }, [value])

    useEffect(() => {
        SetValue(fileList, onChange);
    }, [fileList]);

    return <div>
        <Upload
            action={action}
            accept={accept}
            listType="picture-card"
            fileList={fileList}
            onPreview={async file => {
                if (!file.url && !file.preview) {
                    file.preview = await getBase64(file.originFileObj);
                }

                setPreviewImage(file.preview || file.url || '')
                setPreviewVisible(true);
                setPreviewTitle(file.name || getUrlFileName(file.url || '') || '')
            }}
            onChange={UploadOnChange(max, setFileList)}
        >
            {fileList.length >= max ? null : uploadButton}
        </Upload>
        <Modal
            visible={previewVisible}
            title={previewTitle}
            footer={null}
            onCancel={() => {
                setPreviewVisible(false);
            }}
        >
            <img alt="example" style={{ width: '100%' }} src={previewImage} />
        </Modal>
    </div>
}

const UploadCustom: React.FC<UploadCustomProps> = ({
    type,
    action,
    value,
    onChange,
    accept,
    max,
}) => {
    const tempProps = {
        action: action ? defaultConfig.basePath + action : '',
        value,
        onChange,
        accept,
        max,
    }
    return type === 'image' ? <UploadImageDOM {...tempProps} /> : <UploadFileDOM {...tempProps} />
}

export default UploadCustom;

function SetValue(list: UploadFile<any>[], onChange?: (value: string) => void) {
    const changeValue = (list.filter(temp => temp.status === 'done' && temp.url).map(temp => temp.url?.replace(defaultConfig.basePath, '')) as string[] || []).join(',');
    onChange && onChange(changeValue);
}

function SetUploadValue(value: string, setFileList: React.Dispatch<React.SetStateAction<UploadFile<any>[]>>, fileList: UploadFile<any>[]) {
    const fieldData = value.split(',').filter(temp => !!temp);
    setFileList(fieldData.map((temp, index): UploadFile<any> => {
        return {
            uid: fileList.find(uploadFile => {
                return uploadFile.url?.replace(defaultConfig.basePath, '') === temp;
            })?.uid || index.toString(),
            status: 'done',
            url: defaultConfig.basePath + temp,
            name: getUrlFileName(temp),
            size: 0,
            type: ''
        };
    }));
}

function UploadOnChange(max: number, setFileList: React.Dispatch<React.SetStateAction<UploadFile<any>[]>>) {
    return ({ file, fileList }: UploadChangeParam<UploadFile<any>>) => {
        if (file.status === 'done') {
            const res = file.response as HandleResult<string>;
            if (res.isSuccess) {
                file.url = res.data;
                if (max === 1) {
                    setFileList([file]);
                    return;
                }

                fileList.forEach(item => {
                    if (item.uid == file.uid) {
                        item.url = file.url;
                    }
                });
            } else {
                fileList.forEach(item => {
                    if (item.uid == file.uid) {
                        item.status = 'error';
                        item.response = res.message;
                    }
                });
            }
        }
        setFileList(fileList);
    };
}

