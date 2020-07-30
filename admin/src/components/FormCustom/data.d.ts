
export interface UploadCustomProps {
    type: 'image' | 'file';
    action?: string;
    max?: number;
    value?: string;
    accept?: string;
    onChange?: (value: string) => void;
}