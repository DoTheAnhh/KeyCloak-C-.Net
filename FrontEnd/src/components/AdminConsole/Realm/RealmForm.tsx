import React, { useCallback, useEffect, useState } from 'react'
import axios from 'axios';
import { LOCALHOST, MAPPING_URL } from '../../../api/API';
import { RealmRequest } from '../../../Interface/Realm';
import { Button, Col, Form, Input, Popconfirm, Row, Switch } from 'antd';

interface RealmProps {
    handleCancelRealmModal: () => void;
    realmSelected: string | null;
}

const RealmForm: React.FC<RealmProps> = ({ handleCancelRealmModal, realmSelected }) => {

    const [realm, setRealm] = useState<RealmRequest>()

    const findRealmById = async () => {
        try {
            if (realmSelected) {
                const res = await axios.get(LOCALHOST + MAPPING_URL.REALM + `/${realmSelected}`);
                setRealm(res.data);
            }
        } catch (error) {
            console.error("Error fetching realm by ID:", error);
        }
    };

    const handleChangeSingleField = useCallback(
        (field: string) => {
            return (value: string | number) => {
                setRealm((prev) => ({
                    ...prev,
                    [field]: value,
                }));
            };
        },
        []
    );

    const handleInsertOrUpdate = async () => {
        try {
            if (realmSelected) {
                await axios.put(LOCALHOST + MAPPING_URL.REALM + `/${realmSelected}`, realm)
            } else {
                await axios.post(LOCALHOST + MAPPING_URL.REALM, realm)
            }
            handleCancelRealmModal()
        } catch (err) { }
    }

    const clearField = () => {
        setRealm(undefined)
    }

    useEffect(() => {
        findRealmById()
        clearField()
    }, [realmSelected])

    return (
        <Form layout="vertical">
            <Row gutter={16}>
                <Col span={12}>
                    <Form.Item label="Realm" required>
                        <Input
                            value={realm?.realm}
                            onChange={(e) => handleChangeSingleField("realm")(e.target.value)}
                        ></Input>
                    </Form.Item>
                    <Form.Item label="Display name" required>
                        <Input
                            value={realm?.displayName}
                            onChange={(e) => handleChangeSingleField("displayName")(e.target.value)}
                        ></Input>
                    </Form.Item>
                    <Form.Item label="Enable" required>
                        <Switch
                            checked={realm?.enabled} 
                            onChange={(checked: any) => handleChangeSingleField("enabled")(checked)}
                        />
                    </Form.Item>
                    <Form.Item>
                        <Popconfirm
                            title="Are you sure you want to submit ?"
                            onConfirm={() => handleInsertOrUpdate()}
                            okText="Yes"
                            cancelText="No"
                        >
                            <Button type="primary">Submit</Button>
                        </Popconfirm>
                    </Form.Item>
                </Col>
            </Row>
        </Form>
    )
}

export default RealmForm