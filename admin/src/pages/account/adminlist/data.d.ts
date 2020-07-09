import { ModalBase } from '@/components/ListTable';

export interface AdminListProps {

}

export interface Administrator extends ModalBase {
    accountName: string;
    groupNum: string | string[]
}