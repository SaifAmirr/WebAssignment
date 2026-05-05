import api from './api';

export const getAllStudents = () => api.get('/student');
export const getStudentById = (id) => api.get(`/student/${id}`);
export const createStudent = (data) => api.post('/student', data);
export const updateStudent = (id, data) => api.put(`/student/${id}`, data);
export const withdrawFromCourse = (studentId, courseId) =>
  api.delete(`/student/${studentId}/withdraw/${courseId}`);
