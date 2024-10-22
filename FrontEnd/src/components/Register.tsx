import { useState } from 'react';
import './Register.css'; // Tạo file CSS riêng nếu cần
import axios from 'axios';
import { useNavigate } from 'react-router-dom';

const Register: React.FC = () => {
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [confirmPassword, setConfirmPassword] = useState('');
    const [error, setError] = useState('');
    const [loading, setLoading] = useState(false);

    const navigate = useNavigate();

    const onRegister = async () => {
        if (password !== confirmPassword) {
            setError('Mật khẩu và xác nhận mật khẩu không khớp.');
            return;
        }

        try {
            setLoading(true);
            await axios.post("https://localhost:44333/api/auth/register", {
                username,
                password,
            });
            setLoading(false);
            setError('');
            alert('Đăng ký thành công!');
            navigate("/login");
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

        if (username === '' || password === '' || confirmPassword === '') {
            setError('Vui lòng nhập đầy đủ thông tin.');
        } else {
            setError('');
            console.log('Đăng ký với', { username, password });
            onRegister();
        }
    };

    return (
        <div className="register-container">
            <h2>Đăng Ký</h2>
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
                <div className="form-group pass">
                    <label htmlFor="confirmPassword" className="passlabel1">Pass confirm:</label>
                    <input
                        type="password"
                        id="confirmPassword"
                        value={confirmPassword}
                        onChange={(e) => setConfirmPassword(e.target.value)}
                        required
                        className="passinput1"
                        disabled={loading} // Không cho nhập khi đang xử lý
                    />
                </div>
                {error && <p className="error-message">{error}</p>}
                <button type="submit" disabled={loading}>
                    {loading ? 'Đang đăng ký...' : 'Đăng Ký'}
                </button>
            </form>
        </div>
    );
};

export default Register;
