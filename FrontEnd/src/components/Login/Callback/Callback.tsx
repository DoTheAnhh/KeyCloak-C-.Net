import { Button, message } from 'antd';
import axios from 'axios';
import React, { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';

const Callback: React.FC = () => {

    const navigator = useNavigate();

    const exchangeCodeForToken = async (code: string) => {
        const response = await axios.post(`https://localhost:44333/api/auth/exchange?code=${code}`);
        const accessToken = response.data.accessToken;
        const refreshToken = response.data.refreshToken;

        localStorage.setItem('token', accessToken);
        localStorage.setItem('refreshToken', refreshToken);
        localStorage.setItem('realm', 'DemoRealm');

        message.success('Đăng nhập thành công!');
        navigator("/user");
    };

    useEffect(() => {
        const params = new URLSearchParams(window.location.search);
        const code = params.get('code');

        if (code) {
            exchangeCodeForToken(code);
        }
    }, []);

    return (
        <div
            style={{
                display: 'flex',
                justifyContent: 'center',
                alignItems: 'center',
                height: '100vh',
                backgroundColor: '#f0f2f5',
            }}
        >
            <Button type="primary" onClick={() => navigator("/user")}>
                Tiếp tục
            </Button>
        </div>
    );
};

export default Callback;
