import React, { useCallback, useEffect, useState } from 'react'
import { LOCALHOST, MAPPING_URL } from '../../../api/API';
import axios from 'axios';
import { UserRequest } from '../../../Interface/User';
import { Button, Col, Form, Input, Popconfirm, Row } from 'antd';

interface UserProps {
    handleCancelUserModal: () => void;
    userSelected: string | null;
    realmSelected: string | null;
}

const UserForm: React.FC<UserProps> = ({ handleCancelUserModal, userSelected, realmSelected }) => {

    const [user, setUser] = useState<UserRequest>()

    const findUserById = async () => {
        try {
            if (userSelected) {
                const res = await axios.get(LOCALHOST + MAPPING_URL.USER + `/${userSelected}?realm=${realmSelected}`);
                setUser(res.data);
            }
        } catch (error) {
            console.error("Error fetching realm by ID:", error);
        }
    };

    const handleChangeSingleField = useCallback(
        (field: string) => {
            return (value: string | number) => {
                setUser((prev) => ({
                    ...prev,
                    [field]: value,
                }));
            };
        },
        []
    );

    const handleInsertOrUpdate = async () => {
        try {
            if (userSelected) {
                await axios.put(LOCALHOST + MAPPING_URL.USER + `/${userSelected}?realm=${realmSelected}`, user)
            } else {
                await axios.post(LOCALHOST + MAPPING_URL.USER + `?realm=${realmSelected}`, user)
            }
            handleCancelUserModal()
        } catch (err) { }
    }

    const clearField = () => {
        setUser(undefined)
    }

    useEffect(() => {
        findUserById()
        clearField()
    }, [userSelected])

    return (
        <>
            <Form layout="vertical">
                <Row gutter={16}>
                    <Col span={12}>
                        <Form.Item label="Username" required>
                            <Input
                                value={user?.username}
                                onChange={(e) => handleChangeSingleField("username")(e.target.value)}
                            ></Input>
                        </Form.Item>
                        {/* <Form.Item label="Email" required>
                            <Input
                                value={user?.email}
                                onChange={(e) => handleChangeSingleField("email")(e.target.value)}
                            ></Input>
                        </Form.Item> */}
                        {/* <Form.Item label="Email verified" required>
                            <Switch
                                checked={user?.emailVerified}
                                onChange={(checked: any) => handleChangeSingleField("emailVerified")(checked)}
                            />
                        </Form.Item> */}
                        {/* <Form.Item label="Enable" required>
                            <Switch
                                checked={user?.enabled}
                                onChange={(checked: any) => handleChangeSingleField("enabled")(checked)}
                            />
                        </Form.Item> */}
                        <Form.Item label="First name" required>
                            <Input
                                value={user?.firstName}
                                onChange={(e) => handleChangeSingleField("firstName")(e.target.value)}
                            ></Input>
                        </Form.Item>
                        <Form.Item label="Last name" required>
                            <Input
                                value={user?.lastName}
                                onChange={(e) => handleChangeSingleField("lastName")(e.target.value)}
                            ></Input>
                        </Form.Item>
                        {/* <Form.Item label="Required actions" required>
                            <Input
                                value={user?.requiredActions}
                                onChange={(e) => handleChangeSingleField("requiredActions")(e.target.value)}
                            ></Input>
                        </Form.Item> */}
                        {/* <Form.Item label="Groups" required>
                            <Input
                                value={user?.groups}
                                onChange={(e) => handleChangeSingleField("groups")(e.target.value)}
                            ></Input>
                        </Form.Item> */}
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
        </>
    )
}

export default UserForm