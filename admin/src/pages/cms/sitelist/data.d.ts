import { ModalBase } from '@/components/ListTable';

export interface SiteListProps {

}

export interface Site extends ModalBase {
    host: string | string[];
}