import { useEffect, useState } from 'react';
import './Login.css';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';
import { RealmResponse } from '../../Interface/Realm';
import { LOCALHOST, MAPPING_URL } from '../../api/API';

const Login: React.FC = () => {
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');
    const [loading, setLoading] = useState(false);

    const [realms, setRealms] = useState<RealmResponse[]>([]);
    const [selectedRealm, setSelectedRealm] = useState<string>('');

    const navigate = useNavigate()

    const getAllRealms = async () => {
        try {
            const res = await axios.get(LOCALHOST + MAPPING_URL.REALM);
            setRealms(res.data);
        } catch (err) {
            console.error("Error fetching realms:", err);
        }
    };

    const onLogin = async () => {
        try {
            setLoading(true);
            const response = await axios.post(`https://localhost:44333/api/auth/login?realm=${selectedRealm}`, {
                username,
                password,
            });
            setLoading(false);
            localStorage.setItem('token', response.data.access_token);
            setError('');
            navigate("/admin")
        } catch (err: any) {
            setLoading(false);
            if (err.response && err.response.data && err.response.data.message) {
                setError(err.response.data.message);
            } else {
                setError('Đã xảy ra lỗi. Vui lòng thử lại.');
            }
        }
    };

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();

        if (username === '' || password === '') {
            setError('Vui lòng nhập tên người dùng và mật khẩu.');
        } else {
            setError('');
            console.log('Đăng nhập với', { username, password });
            onLogin();
        }
    };

    useEffect(() => {
        getAllRealms();
    }, []);

    return (
        <div className="login-container">
            <h2>Đăng Nhập</h2>
            <form onSubmit={handleSubmit}>
                <div className="form-group">
                    <label htmlFor="username" className="usernamelabel">Username:</label>
                    <input
                        type="text"
                        id="username"
                        value={username}
                        onChange={(e) => setUsername(e.target.value)}
                        required
                        className="usernameinput"
                        disabled={loading} // Không cho nhập khi đang xử lý
                    />
                </div>
                <div className="form-group pass">
                    <label htmlFor="password" className="passlabel">Password:</label>
                    <input
                        type="password"
                        id="password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        required
                        className="passinput"
                        disabled={loading} // Không cho nhập khi đang xử lý
                    />
                </div>
                <div className="form-group">
                    <label htmlFor="realm" className="realmLabel">Realm:</label>
                    <select
                        id="realm"
                        value={selectedRealm}
                        onChange={(e) => setSelectedRealm(e.target.value)}
                        required
                        disabled={loading} // Không cho nhập khi đang xử lý
                        className="realmSelect"
                    >
                        <option value="">Chọn Realm</option>
                        {realms.map(realm => (
                            <option key={realm.realm} value={realm.realm}>{realm.realm}</option>
                        ))}
                    </select>
                </div>
                {error && <p className="error-message">{error}</p>}
                <button type="submit" disabled={loading}>
                    {loading ? 'Đang đăng nhập...' : 'Đăng Nhập'}
                </button>
            </form>
        </div>
    );
};

export default Login;
