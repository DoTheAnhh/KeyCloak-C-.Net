import React, { useEffect, useState } from 'react';
import { RealmResponse } from '../../../Interface/Realm';
import axios from 'axios';
import { LOCALHOST, MAPPING_URL } from '../../../api/API';
import { Button, Modal, Space, Table } from 'antd';
import type { ColumnsType } from 'antd/es/table';
import { EditOutlined } from '@ant-design/icons';
import RealmForm from './RealmForm';

const ListRealm: React.FC = () => {
    const [realms, setRealms] = useState<RealmResponse[]>([]);

    const [isOpenRealmModal, setIsOpenRealmModal] = useState(false);
    const [realmSelected, setRealmSelected] = useState<string | null>(null);

    const getAllRealms = async () => {
        try {
            const res = await axios.get(LOCALHOST + MAPPING_URL.REALM);
            setRealms(res.data);
        } catch (err) {
            console.error("Error fetching realms:", err);
        }
    };

    const showRealmModal = () => {
        setRealmSelected(null);
        setIsOpenRealmModal(true);
    };

    const showRealmModalById = (record: RealmResponse) => {
        setRealmSelected(record.realm);
        setIsOpenRealmModal(true);
    };

    const handleRealmModalCancel = () => {
        setIsOpenRealmModal(false);
        setRealmSelected(null);
        getAllRealms();
    };

    const columns: ColumnsType<RealmResponse> = [
        {
            title: 'Realm',
            dataIndex: 'realm',
            key: 'realm',
            align: 'center',
        },
        {
            title: 'Display Name',
            dataIndex: 'displayName',
            key: 'displayName',
            align: 'center',
        },
        {
            title: 'Enabled',
            dataIndex: 'enabled',
            key: 'enabled',
            align: 'center',
            render: (text: boolean) => (text ? 'Enabled' : 'Disabled'),
        },
        {
            title: 'Action',
            key: 'action',
            align: 'center',
            render: (record: RealmResponse) => (
                <Space size="middle">
                    <EditOutlined
                        style={{ color: '#1890ff', cursor: 'pointer' }}
                        onClick={() => showRealmModalById(record)}
                    />
                </Space>
            ),
        },
    ];

    useEffect(() => {
        getAllRealms();
    }, []);

    return (
        <>
            <Modal
                open={isOpenRealmModal}
                onCancel={handleRealmModalCancel}
                footer={null}
            >
                <RealmForm
                    handleCancelRealmModal={handleRealmModalCancel}
                    realmSelected={realmSelected}
                />
            </Modal>
            <Button type='primary' style={{ float: 'right', margin: "10px" }} onClick={showRealmModal}>
                New realm +
            </Button>
            <Table<RealmResponse>
                columns={columns}
                dataSource={realms}
                rowKey="realm"
                pagination={false}
            />
        </>
    );
};

export default ListRealm;
